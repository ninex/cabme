package za.co.cabme.android;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.List;
import java.util.Locale;

import org.apache.http.HttpEntity;
import org.apache.http.HttpResponse;
import org.apache.http.StatusLine;
import org.apache.http.client.ClientProtocolException;
import org.apache.http.client.HttpClient;
import org.apache.http.client.methods.HttpGet;
import org.apache.http.impl.client.DefaultHttpClient;

import com.google.android.maps.GeoPoint;

import android.app.ActionBar;
import android.app.Activity;
import android.app.AlertDialog;
import android.app.DatePickerDialog;
import android.app.Dialog;
import android.app.TimePickerDialog;
import android.content.DialogInterface;
import android.content.Intent;
import android.location.Address;
import android.location.Geocoder;
import android.location.Location;
import android.os.AsyncTask;
import android.os.Bundle;
import android.text.InputType;
import android.util.Log;
import android.view.Menu;
import android.view.MenuInflater;
import android.view.MenuItem;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.View.OnLongClickListener;
import android.widget.DatePicker;
import android.widget.EditText;
import android.widget.LinearLayout;
import android.widget.TextView;
import android.widget.TimePicker;
import android.widget.Toast;

public class BookActivity extends Activity {
	public static final String NEWBOOKING_FLAG = "NEWBOOKING_FLAG";
	public static final int PICK_ADDRESS_FROM = 11;
	public static final int PICK_ADDRESS_TO = 12;
	static final int TIME_DIALOG_ID = 0;
	static final int DATEPICKER_DIALOG_ID = 1;
	static final int NUMPEOPLE_DIALOG_ID = 2;

	boolean newBooking = false;

	private TextView mTimeDisplay, mDateDisplay, txtNumPeople, txtDistance;

	private int mHour, mMinute, mDay, mMonth, mYear, mNumPeople;

	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.book);
		getActionBar().setDisplayOptions(
				ActionBar.DISPLAY_SHOW_HOME | ActionBar.DISPLAY_HOME_AS_UP
						| ActionBar.DISPLAY_SHOW_TITLE);
		Bundle b = getIntent().getExtras();
		if (b != null) {
			newBooking = b.getBoolean(NEWBOOKING_FLAG);
			if (newBooking) {
				getActionBar().setTitle("Make Booking");
			} else {
				getActionBar().setTitle("View Booking");
			}
		}
		LinearLayout btnFrom = (LinearLayout) findViewById(R.id.btnFrom);
		btnFrom.setOnClickListener(mMapFromListener);
		btnFrom.setOnLongClickListener(mMapFromLongListener);
		LinearLayout btnTo = (LinearLayout) findViewById(R.id.btnTo);
		btnTo.setOnClickListener(mMapToListener);
		btnTo.setOnLongClickListener(mMapToLongListener);

		// capture our View elements
		mTimeDisplay = (TextView) findViewById(R.id.timeDisplay);
		mDateDisplay = (TextView) findViewById(R.id.dateDisplay);
		txtNumPeople = (TextView) findViewById(R.id.txtNumPeople);
		txtDistance = (TextView) findViewById(R.id.txtDistance);
		LinearLayout mPickTime = (LinearLayout) findViewById(R.id.pickTime);
		LinearLayout mPickDate = (LinearLayout) findViewById(R.id.pickDate);
		LinearLayout pickNumPeople = (LinearLayout) findViewById(R.id.pickNumPeople);
		LinearLayout btnDistance = (LinearLayout) findViewById(R.id.btnDistance);

		// add a click listener to the button
		mPickTime.setOnClickListener(new View.OnClickListener() {
			@SuppressWarnings("deprecation")
			public void onClick(View v) {
				showDialog(TIME_DIALOG_ID);
			}
		});
		mPickDate.setOnClickListener(new View.OnClickListener() {
			@SuppressWarnings("deprecation")
			public void onClick(View v) {
				showDialog(DATEPICKER_DIALOG_ID);
			}
		});
		pickNumPeople.setOnClickListener(new View.OnClickListener() {
			@SuppressWarnings("deprecation")
			public void onClick(View v) {
				showDialog(NUMPEOPLE_DIALOG_ID);
			}
		});
		btnDistance.setOnClickListener(new View.OnClickListener() {
			public void onClick(View v) {
				Intent intent = new Intent(BookActivity.this, ShowMapActivity.class);
				Bundle b = new Bundle();
		    	b.putString(ShowMapActivity.TOADDR_FLAG, ((TextView) findViewById(R.id.txtAddressTo)).getText().toString());
		    	b.putString(ShowMapActivity.FROMADDR_FLAG, ((TextView) findViewById(R.id.txtAddressFrom)).getText().toString());
		    	b.putBoolean(ShowMapActivity.KEY_ADDRESS_LOCKED, true);
		    	intent.putExtras(b);
				startActivity(intent);
			}
		});

		// get the current time
		final Calendar c = Calendar.getInstance();
		mHour = c.get(Calendar.HOUR_OF_DAY);
		mMinute = c.get(Calendar.MINUTE);
		mYear = c.get(Calendar.YEAR);
		mMonth = c.get(Calendar.MONTH);
		mDay = c.get(Calendar.DAY_OF_MONTH);
		mNumPeople = 1;

		// display the current date
		updateDisplay();

	}

	// the callback received when the user "sets" the time in the dialog
	private TimePickerDialog.OnTimeSetListener mTimeSetListener = new TimePickerDialog.OnTimeSetListener() {
		public void onTimeSet(TimePicker view, int hourOfDay, int minute) {
			mHour = hourOfDay;
			mMinute = minute;
			updateDisplay();
		}
	};

	// the callback received when the user "sets" the date in the dialog
	private DatePickerDialog.OnDateSetListener mDateSetListener = new DatePickerDialog.OnDateSetListener() {

		public void onDateSet(DatePicker view, int year, int monthOfYear,
				int dayOfMonth) {
			mYear = year;
			mMonth = monthOfYear;
			mDay = dayOfMonth;
			updateDisplay();
		}
	};

	// updates the date in the TextView
	private void updateDisplay() {
		mTimeDisplay.setText(new StringBuilder().append(pad(mHour)).append(":")
				.append(pad(mMinute)));
		mDateDisplay.setText(new StringBuilder()
				// Month is 0 based so add 1
				.append(pad(mDay)).append("-").append(pad(mMonth + 1))
				.append("-").append(mYear).append(" "));
		txtNumPeople.setText(new StringBuilder().append(mNumPeople));
	}

	private void updateAddresses(String from, String to) {
		TextView txtFrom = (TextView) findViewById(R.id.txtAddressFrom);
		TextView txtTo = (TextView) findViewById(R.id.txtAddressTo);
		if (from != null && !from.equals("")
				&& !from.equals(this.getString(R.string.map_loading_addr))) {
			txtFrom.setText(from);
		}
		if (to != null && !to.equals("")
				&& !to.equals(this.getString(R.string.map_loading_addr))) {
			txtTo.setText(to);
		}
		// We have a route!
		if (txtFrom.getText().length() > 0 && txtTo.getText().length() > 0) {
			txtDistance.setText(this.getString(R.string.map_calculating_dist));
			String[] list = new String[2];
			list[0] = txtFrom.getText().toString();
			list[1] = txtTo.getText().toString();

			new AsyncTask<String[], Void, List<GeoPoint>>() {
				@Override
				protected List<GeoPoint> doInBackground(String[]... addresses) {
					List<GeoPoint> poly = new ArrayList<GeoPoint>();
					try {
						Geocoder geoCoder = new Geocoder(getBaseContext(),
								Locale.getDefault());
						Address fromAddr = geoCoder.getFromLocationName(
								addresses[0][0], 1).get(0);
						Address toAddr = geoCoder.getFromLocationName(
								addresses[0][1], 1).get(0);
						/*
						 * double latitude = geoPoints[0].getLatitudeE6() / 1E6;
						 * double longitude = geoPoints[0].getLongitudeE6() /
						 * 1E6;
						 */

						String encoded = queryRESTurl(getUrl(
								fromAddr.getLatitude(),
								fromAddr.getLongitude(), toAddr.getLatitude(),
								toAddr.getLongitude()));
						// get only the encoded geopoints
						encoded = encoded.split("points:\"")[1].split("\",")[0];
						// replace two backslashes by one (some error from the
						// transmission)
						encoded = encoded.replace("\\\\", "\\");

						// decoding

						int index = 0, len = encoded.length();
						int lat = 0, lng = 0;

						while (index < len) {
							int b, shift = 0, result = 0;
							do {
								b = encoded.charAt(index++) - 63;
								result |= (b & 0x1f) << shift;
								shift += 5;
							} while (b >= 0x20);
							int dlat = ((result & 1) != 0 ? ~(result >> 1)
									: (result >> 1));
							lat += dlat;

							shift = 0;
							result = 0;
							do {

								b = encoded.charAt(index++) - 63;
								result |= (b & 0x1f) << shift;
								shift += 5;
							} while (b >= 0x20);
							int dlng = ((result & 1) != 0 ? ~(result >> 1)
									: (result >> 1));
							lng += dlng;

							GeoPoint p = new GeoPoint(
									(int) (((double) lat / 1E5) * 1E6),
									(int) (((double) lng / 1E5) * 1E6));
							poly.add(p);
						}

					} catch (IOException ex) {
						ex.printStackTrace();
					}
					return poly;
				}

				@Override
				protected void onPostExecute(List<GeoPoint> poly) {
					if (poly != null && poly.size() > 0) {
						float distance = 0;
						float[] results = new float[1];
						/*
						 * Log.i(BookActivity.class.getName(),
						 * "Amount of points = " + poly.size());
						 */
						for (int i = 0; i <= poly.size() - 2; i++) {
							Location.distanceBetween(poly.get(i)
									.getLatitudeE6() / 1E6, poly.get(i)
									.getLongitudeE6() / 1E6, poly.get(i + 1)
									.getLatitudeE6() / 1E6, poly.get(i + 1)
									.getLongitudeE6() / 1E6, results);
							if (results != null && results.length >= 0) {
								distance += results[results.length - 1];
								/*
								 * Log.i(BookActivity.class.getName(),
								 * "Distance between " +
								 * poly.get(i).getLatitudeE6()/ 1E6 + "," +
								 * poly.get(i).getLongitudeE6()/ 1E6 + " and " +
								 * poly.get(i + 1).getLatitudeE6()/ 1E6+ "," +
								 * poly.get(i + 1).getLongitudeE6() / 1E6+
								 * " is " + results[results.length - 1]);
								 */
							}
						}

						if (distance < 1000) {
							txtDistance.setText(new StringBuilder().append(
									Math.ceil(distance)).append(" m"));
						} else {
							txtDistance.setText(new StringBuilder().append(
									Math.ceil(distance)/1000).append(" km"));
						}
					}
				}
			}.execute(list);
		}
	}

	// http://blog.synyx.de/2010/06/routing-driving-directions-on-android-part-1-get-the-route/
	public String getUrl(double srcLat, double srcLong, double destLat,
			double destLong) {

		StringBuilder urlString = new StringBuilder();

		urlString.append("http://maps.google.com/maps?f=d&hl=en");
		urlString.append("&saddr=");
		urlString.append(Double.toString(srcLat));
		urlString.append(",");
		urlString.append(Double.toString(srcLong));
		urlString.append("&daddr=");// to
		urlString.append(Double.toString(destLat));
		urlString.append(",");
		urlString.append(Double.toString(destLong));
		urlString.append("&ie=UTF8&0&om=0&output=dragdir");

		return urlString.toString();
	}

	public String queryRESTurl(String url) {
		StringBuilder builder = new StringBuilder();
		HttpClient client = new DefaultHttpClient();
		HttpGet httpGet = new HttpGet(url);
		try {
			HttpResponse response = client.execute(httpGet);
			StatusLine statusLine = response.getStatusLine();
			int statusCode = statusLine.getStatusCode();
			if (statusCode == 200) {
				HttpEntity entity = response.getEntity();
				InputStream content = entity.getContent();
				BufferedReader reader = new BufferedReader(
						new InputStreamReader(content));
				String line;
				while ((line = reader.readLine()) != null) {
					builder.append(line);
				}
			} else {
				Log.e(BookActivity.class.toString(), "Failed to download file");
			}
		} catch (ClientProtocolException e) {
			e.printStackTrace();
		} catch (IOException e) {
			e.printStackTrace();
		}
		return builder.toString();
	}

	private void updateFromAddress(String address) {
		updateAddresses(address, null);
	}

	private void updateToAddress(String address) {
		updateAddresses(null, address);
	}

	private static String pad(int c) {
		if (c >= 10)
			return String.valueOf(c);
		else
			return "0" + String.valueOf(c);
	}

	@Override
	protected Dialog onCreateDialog(int id) {
		switch (id) {
		case TIME_DIALOG_ID:
			return new TimePickerDialog(this, mTimeSetListener, mHour, mMinute,
					false);
		case DATEPICKER_DIALOG_ID:
			return new DatePickerDialog(this, mDateSetListener, mYear, mMonth,
					mDay);
		case NUMPEOPLE_DIALOG_ID:
			final CharSequence[] items = { "1", "2", "3", "4", "5", "6", "7",
					"8", "9", "10" };

			AlertDialog.Builder builder = new AlertDialog.Builder(this);
			builder.setTitle("Number of people");
			builder.setItems(items, new DialogInterface.OnClickListener() {
				public void onClick(DialogInterface dialog, int item) {
					mNumPeople = Integer.parseInt(items[item].toString());
					updateDisplay();
				}
			});
			AlertDialog alert = builder.create();
			return alert;
		}

		return null;
	}

	@Override
	public boolean onCreateOptionsMenu(Menu menu) {
		MenuInflater inflater = getMenuInflater();
		inflater.inflate(R.menu.mapmenu, menu);
		return true;
	}

	@Override
	public boolean onOptionsItemSelected(MenuItem item) {
		switch (item.getItemId()) {
		case android.R.id.home:
			Intent intent = new Intent(this, CabmeActivity.class);
			intent.addFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP);
			startActivity(intent);
			return true;
		case R.id.menu_Save:
			if (newBooking) {
				makeBooking();
			}
			finish();
			return true;
		case R.id.menu_Cancel:
			if (!newBooking) {
				cancelBooking();
			}
			finish();
			return true;
		default:
			return super.onOptionsItemSelected(item);
		}
	}

	private void makeBooking() {
		Toast.makeText(BookActivity.this, "Booking created", Toast.LENGTH_SHORT)
				.show();
	}

	private void cancelBooking() {
		Toast.makeText(BookActivity.this, "Booking cancelled",
				Toast.LENGTH_SHORT).show();
	}

	private OnClickListener mMapFromListener = new OnClickListener() {
		public void onClick(View view) {
			Intent intent = new Intent(BookActivity.this, ShowMapActivity.class);
			Bundle b = new Bundle();
	    	b.putString(ShowMapActivity.FROMADDR_FLAG, ((TextView) findViewById(R.id.txtAddressFrom)).getText().toString());
	    	b.putString(ShowMapActivity.TOADDR_FLAG, ((TextView) findViewById(R.id.txtAddressTo)).getText().toString());
	    	intent.putExtras(b);
			startActivityForResult(intent, PICK_ADDRESS_FROM);
		}
	};

	private OnLongClickListener mMapFromLongListener = new OnLongClickListener() {
		public boolean onLongClick(View v) {
			final EditText inputFrom = new EditText(BookActivity.this);
			AlertDialog.Builder fromAlert = new AlertDialog.Builder(
					BookActivity.this)
					.setTitle("Update From Address")
					.setView(inputFrom)
					.setPositiveButton("Ok",
							new DialogInterface.OnClickListener() {
								public void onClick(DialogInterface dialog,
										int whichButton) {
									updateFromAddress(inputFrom.getText()
											.toString());
								}
							})
					.setNegativeButton("Cancel",
							new DialogInterface.OnClickListener() {
								public void onClick(DialogInterface dialog,
										int whichButton) {
									// Do nothing.
								}
							});
			TextView txtFrom = (TextView) findViewById(R.id.txtAddressFrom);
			inputFrom.setText(txtFrom.getText());
			inputFrom.setInputType(InputType.TYPE_TEXT_VARIATION_POSTAL_ADDRESS
					| InputType.TYPE_TEXT_FLAG_MULTI_LINE);
			fromAlert.show();
			return true;
		}
	};

	private OnClickListener mMapToListener = new OnClickListener() {
		public void onClick(View view) {
			Intent intent = new Intent(BookActivity.this, ShowMapActivity.class);
			Bundle b = new Bundle();
	    	b.putString(ShowMapActivity.TOADDR_FLAG, ((TextView) findViewById(R.id.txtAddressTo)).getText().toString());
	    	b.putString(ShowMapActivity.FROMADDR_FLAG, ((TextView) findViewById(R.id.txtAddressFrom)).getText().toString());
	    	intent.putExtras(b);
			startActivityForResult(intent, PICK_ADDRESS_TO);
		}
	};

	private OnLongClickListener mMapToLongListener = new OnLongClickListener() {
		public boolean onLongClick(View v) {
			final EditText inputTo = new EditText(BookActivity.this);
			AlertDialog.Builder toAlert = new AlertDialog.Builder(
					BookActivity.this)
					.setTitle("Update To Address")
					.setView(inputTo)
					.setPositiveButton("Ok",
							new DialogInterface.OnClickListener() {
								public void onClick(DialogInterface dialog,
										int whichButton) {
									updateToAddress(inputTo.getText()
											.toString());
								}
							})
					.setNegativeButton("Cancel",
							new DialogInterface.OnClickListener() {
								public void onClick(DialogInterface dialog,
										int whichButton) {
									// Do nothing.
								}
							});
			TextView txtTo = (TextView) findViewById(R.id.txtAddressTo);
			inputTo.setText(txtTo.getText());
			inputTo.setInputType(InputType.TYPE_TEXT_VARIATION_POSTAL_ADDRESS
					| InputType.TYPE_TEXT_FLAG_MULTI_LINE);
			toAlert.show();
			return true;
		}
	};

	protected void onActivityResult(int requestCode, int resultCode, Intent data) {
		if (newBooking) {
			if (requestCode == PICK_ADDRESS_FROM) {
				if (resultCode == RESULT_OK) {
					updateFromAddress(data.getCharSequenceExtra(
							ShowMapActivity.KEY_ADDRESS_FROM).toString());
				}
			}
			if (requestCode == PICK_ADDRESS_TO) {
				if (resultCode == RESULT_OK) {
					updateToAddress(data.getCharSequenceExtra(
							ShowMapActivity.KEY_ADDRESS_TO).toString());
				}
			}
		}
	}
}
