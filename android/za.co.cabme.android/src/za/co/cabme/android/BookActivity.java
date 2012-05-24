package za.co.cabme.android;

import java.lang.reflect.Method;
import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.Calendar;
import java.util.Date;

import com.google.android.maps.GeoPoint;
import com.google.gson.Gson;

import android.app.ActionBar;
import android.app.Activity;
import android.app.AlertDialog;
import android.app.DatePickerDialog;
import android.app.Dialog;
import android.app.TimePickerDialog;
import android.content.DialogInterface;
import android.content.Intent;
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

	boolean newBooking = false;
	private TextView mTimeDisplay, mDateDisplay, txtNumPeople, txtDistance,
			txtTaxi;
	private int mHour, mMinute, mDay, mMonth, mYear, mNumPeople;
	private GeoPoint pointFrom, pointTo;
	private MapRoute mapRoute;
	private Entities.Booking booking;

	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.book);
		getActionBar().setDisplayOptions(
				ActionBar.DISPLAY_SHOW_HOME | ActionBar.DISPLAY_HOME_AS_UP
						| ActionBar.DISPLAY_SHOW_TITLE);
		Bundle b = getIntent().getExtras();
		if (b != null) {
			newBooking = b.getBoolean(Common.NEWBOOKING_FLAG);
			if (newBooking) {
				getActionBar().setTitle("Make Booking");
				booking = (new Entities()).new Booking();
			} else {
				getActionBar().setTitle("View Booking");
				String json = b.getString(Common.BOOKING_FLAG, null);
				if (json != null) {
					Gson g = new Gson();
					booking = g.fromJson(json, Entities.Booking.class);
				} else {
					booking = (new Entities()).new Booking();
				}
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
		txtTaxi = (TextView) findViewById(R.id.txtTaxi);
		LinearLayout mPickTime = (LinearLayout) findViewById(R.id.pickTime);
		LinearLayout mPickDate = (LinearLayout) findViewById(R.id.pickDate);
		LinearLayout pickNumPeople = (LinearLayout) findViewById(R.id.pickNumPeople);
		LinearLayout btnDistance = (LinearLayout) findViewById(R.id.btnDistance);
		LinearLayout btnTaxi = (LinearLayout) findViewById(R.id.btnTaxi);

		// add a click listener to the button
		mPickTime.setOnClickListener(new View.OnClickListener() {
			@SuppressWarnings("deprecation")
			public void onClick(View v) {
				showDialog(Common.TIME_DIALOG_ID);
			}
		});
		mPickDate.setOnClickListener(new View.OnClickListener() {
			@SuppressWarnings("deprecation")
			public void onClick(View v) {
				showDialog(Common.DATEPICKER_DIALOG_ID);
			}
		});
		pickNumPeople.setOnClickListener(new View.OnClickListener() {
			@SuppressWarnings("deprecation")
			public void onClick(View v) {
				showDialog(Common.NUMPEOPLE_DIALOG_ID);
			}
		});
		btnDistance.setOnClickListener(new View.OnClickListener() {
			public void onClick(View v) {
				Intent intent = new Intent(BookActivity.this,
						ShowMapActivity.class);
				Bundle b = new Bundle();
				b.putString(Common.TOADDR_FLAG,
						((TextView) findViewById(R.id.txtAddressTo)).getText()
								.toString());
				b.putString(Common.FROMADDR_FLAG,
						((TextView) findViewById(R.id.txtAddressFrom))
								.getText().toString());
				b.putBoolean(Common.ADDRESS_LOCKED_FLAG, true);
				intent.putExtras(b);
				startActivity(intent);
			}
		});
		btnTaxi.setOnClickListener(mTaxiListener);

		// Routing
		setupMapRoute();
		// get the current time
		Calendar c = Calendar.getInstance();
		mNumPeople = 1;
		// Booking
		if (booking != null && booking.Id > 0) {
			setupFromBooking();
			try {
				SimpleDateFormat formatter = new SimpleDateFormat(
						"yy-MM-dd hh:mm:ss");
				Date date = (Date) formatter.parse(booking.PickupTime);
				c.setTime(date);
			} catch (ParseException e) {
			}
		}
		mHour = c.get(Calendar.HOUR_OF_DAY);
		mMinute = c.get(Calendar.MINUTE);
		mYear = c.get(Calendar.YEAR);
		mMonth = c.get(Calendar.MONTH);
		mDay = c.get(Calendar.DAY_OF_MONTH);
		// display the current date
		updateDisplay();
	}

	private void setupMapRoute() {
		Method delegate;
		try {
			delegate = BookActivity.class.getMethod("updateDistance",
					new Class[0]);
			mapRoute = new MapRoute(getBaseContext(), this, delegate);
		} catch (NoSuchMethodException e) {
			e.printStackTrace();
		}
	}

	private void setupFromBooking() {
		if (booking.latitudeFrom != 0 && booking.longitudeFrom != 0) {
			pointFrom = new GeoPoint(booking.latitudeFrom,
					booking.longitudeFrom);
		}
		if (booking.latitudeTo != 0 && booking.longitudeTo != 0) {
			pointTo = new GeoPoint(booking.latitudeTo, booking.longitudeTo);
		}
		updateAddresses(booking.AddrFrom, booking.AddrTo);
		mNumPeople = booking.NumberOfPeople;
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
		String time = new StringBuilder().append(pad(mHour)).append(":")
				.append(pad(mMinute)).toString();
		String date = new StringBuilder().append(mYear).append("-")
				.append(pad(mMonth + 1)).append("-").append(pad(mDay))
				.append(" ").toString();
		mTimeDisplay.setText(time);
		mDateDisplay.setText(date);
		txtNumPeople.setText(new StringBuilder().append(mNumPeople));
		booking.NumberOfPeople = (byte) mNumPeople;
		booking.PickupTime = date + time;
		if (booking.SelectedTaxi != null) {
			txtTaxi.setText(booking.SelectedTaxi.Name);
		}
	}

	private void updateAddresses(String from, String to) {
		Log.i(BookActivity.class.getName(), from + "and to:" + to);
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
		booking.AddrFrom = txtFrom.getText().toString();
		booking.AddrTo = txtTo.getText().toString();
		if (pointFrom != null) {
			booking.latitudeFrom = pointFrom.getLatitudeE6();
			booking.longitudeFrom = pointFrom.getLongitudeE6();
		}
		if (pointTo != null) {
			booking.latitudeTo = pointTo.getLatitudeE6();
			booking.longitudeTo = pointTo.getLongitudeE6();
		}
		// We have a route!
		if (pointFrom != null && pointTo != null) {
			txtDistance.setText(this.getString(R.string.map_calculating_dist));
			mapRoute.calculateRoute(pointFrom, pointTo);
		} else {
			if (txtFrom.getText().length() > 0 && txtTo.getText().length() > 0) {
				txtDistance.setText(this
						.getString(R.string.map_calculating_dist));
				mapRoute.calculateRoute(txtFrom.getText().toString(), txtTo
						.getText().toString());
			}
		}
	}

	public void updateDistance() {
		float distance = mapRoute.getDistance();
		if (distance < 1000) {
			txtDistance.setText(new StringBuilder().append(Math.ceil(distance))
					.append(" m"));
		} else {
			txtDistance.setText(new StringBuilder().append(
					Math.ceil(distance) / 1000).append(" km"));
		}
		booking.ComputedDistance = (int) Math.ceil(distance);
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
		case Common.TIME_DIALOG_ID:
			return new TimePickerDialog(this, mTimeSetListener, mHour, mMinute,
					false);
		case Common.DATEPICKER_DIALOG_ID:
			return new DatePickerDialog(this, mDateSetListener, mYear, mMonth,
					mDay);
		case Common.NUMPEOPLE_DIALOG_ID:
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
			b.putString(Common.FROMADDR_FLAG,
					((TextView) findViewById(R.id.txtAddressFrom)).getText()
							.toString());
			b.putString(Common.TOADDR_FLAG,
					((TextView) findViewById(R.id.txtAddressTo)).getText()
							.toString());
			intent.putExtras(b);
			startActivityForResult(intent, Common.PICK_ADDRESS_FROM);
		}
	};

	private OnClickListener mTaxiListener = new OnClickListener() {
		public void onClick(View view) {
			Intent intent = new Intent(BookActivity.this, TaxiActivity.class);
			Bundle b = new Bundle();
			Gson g = new Gson();
			b.putString(Common.BOOKING_FLAG, g.toJson(booking));
			intent.putExtras(b);
			startActivityForResult(intent, Common.PICK_TAXI);
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
									pointFrom = null;
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
			b.putString(Common.TOADDR_FLAG,
					((TextView) findViewById(R.id.txtAddressTo)).getText()
							.toString());
			b.putString(Common.FROMADDR_FLAG,
					((TextView) findViewById(R.id.txtAddressFrom)).getText()
							.toString());
			intent.putExtras(b);
			startActivityForResult(intent, Common.PICK_ADDRESS_TO);
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
									pointTo = null;
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
		int lat, lon;
		if (newBooking) {
			if (requestCode == Common.PICK_ADDRESS_FROM) {
				if (resultCode == RESULT_OK) {
					updateFromAddress(data.getCharSequenceExtra(
							Common.FROMADDR_FLAG).toString());
					lat = data.getIntExtra(Common.FROMLAT_FLAG, 0);
					lon = data.getIntExtra(Common.FROMLONG_FLAG, 0);
					if (lat != 0 && lon != 0) {
						pointFrom = new GeoPoint(lat, lon);
					} else {
						pointFrom = null;
					}
				}
			}
			if (requestCode == Common.PICK_ADDRESS_TO) {
				if (resultCode == RESULT_OK) {
					updateToAddress(data.getCharSequenceExtra(
							Common.TOADDR_FLAG).toString());
					lat = data.getIntExtra(Common.TOLAT_FLAG, 0);
					lon = data.getIntExtra(Common.TOLONG_FLAG, 0);
					if (lat != 0 && lon != 0) {
						pointTo = new GeoPoint(lat, lon);
					} else {
						pointTo = null;
					}
				}
			}
			if (requestCode == Common.PICK_TAXI) {
				if (resultCode == RESULT_OK) {
					String json = data.getStringExtra(Common.TAXI_FLAG);
					Gson g = new Gson();
					Entities.Taxi taxi = g.fromJson(json, Entities.Taxi.class);
					booking.TaxiId = taxi.Id;
					booking.SelectedTaxi = taxi;
					txtTaxi.setText(taxi.Name);
				}
			}
		}
	}
}
