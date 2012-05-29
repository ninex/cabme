package za.co.cabme.android;

import java.lang.reflect.Method;
import java.text.SimpleDateFormat;
import java.util.Calendar;
import java.util.Date;

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
			txtTaxi, txtAddrFrom, txtAddrTo;
	private int mHour, mMinute, mDay, mMonth, mYear, mNumPeople;
	private MapRoute mapRoute;
	private Entities.Booking booking;

	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.book);
		getActionBar().setDisplayOptions(
				ActionBar.DISPLAY_SHOW_HOME | ActionBar.DISPLAY_HOME_AS_UP
						| ActionBar.DISPLAY_SHOW_TITLE);
		Bundle b;
		if (savedInstanceState == null) {
			b = getIntent().getExtras();
		} else {
			b = savedInstanceState;
		}
		if (b != null) {
			newBooking = b.getBoolean(Common.NEWBOOKING_FLAG);
			String json = b.getString(Common.BOOKING_FLAG, null);
			if (json != null) {
				Gson g = new Gson();
				booking = g.fromJson(json, Entities.Booking.class);
			} else {
				booking = (new Entities()).new Booking();
			}
			if (newBooking) {
				getActionBar().setTitle("Make Booking");
			} else {
				getActionBar().setTitle("View Booking");				
			}
		}

		// capture our View elements
		mTimeDisplay = (TextView) findViewById(R.id.timeDisplay);
		mDateDisplay = (TextView) findViewById(R.id.dateDisplay);
		txtNumPeople = (TextView) findViewById(R.id.txtNumPeople);
		txtDistance = (TextView) findViewById(R.id.txtDistance);
		txtTaxi = (TextView) findViewById(R.id.txtTaxi);
		txtAddrFrom = (TextView) findViewById(R.id.txtAddressFrom);
		txtAddrTo = (TextView) findViewById(R.id.txtAddressTo);
		LinearLayout btnFrom = (LinearLayout) findViewById(R.id.btnFrom);
		LinearLayout btnTo = (LinearLayout) findViewById(R.id.btnTo);
		LinearLayout mPickTime = (LinearLayout) findViewById(R.id.pickTime);
		LinearLayout mPickDate = (LinearLayout) findViewById(R.id.pickDate);
		LinearLayout pickNumPeople = (LinearLayout) findViewById(R.id.pickNumPeople);
		LinearLayout btnDistance = (LinearLayout) findViewById(R.id.btnDistance);
		LinearLayout btnTaxi = (LinearLayout) findViewById(R.id.btnTaxi);

		btnFrom.setOnClickListener(mMapFromListener);
		btnFrom.setOnLongClickListener(mMapFromLongListener);
		btnTo.setOnClickListener(mMapToListener);
		btnTo.setOnLongClickListener(mMapToLongListener);
		mPickTime.setOnClickListener(pickTimeClickListener);
		mPickDate.setOnClickListener(pickDateClickListener);
		pickNumPeople.setOnClickListener(pickNumPeopleClickListener);
		btnDistance.setOnClickListener(btnDistanceClickListener);
		btnTaxi.setOnClickListener(btnTaxiListener);

		// Routing
		setupMapRoute();
		// get the current time
		Calendar c = Calendar.getInstance();
		// Booking
		if (booking != null) {
			setupFromBooking();
			try {
				SimpleDateFormat formatter = new SimpleDateFormat(
						"yy-MM-dd hh:mm:ss");
				Date date = (Date) formatter.parse(booking.PickupTime);
				c.setTime(date);
			} catch (Exception e) {
			}
		}else{
			mNumPeople = 1;
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
		updateAddress();
		mNumPeople = booking.NumberOfPeople;
		((TextView) findViewById(R.id.txtPrice)).setText(booking.getPriceEstimate());
	}

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
		((TextView) findViewById(R.id.txtPrice)).setText(booking.getPriceEstimate());
	}

	private void updateAddress() {
		txtAddrFrom.setText(booking.AddrFrom);
		txtAddrTo.setText(booking.AddrTo);
		txtDistance.setText(displayedDistance(booking.ComputedDistance));
		// check if we have a route
		if (booking.getFromPoint() != null && booking.getToPoint() != null) {
			txtDistance.setText(this.getString(R.string.map_calculating_dist));
			mapRoute.calculateRoute(booking.getFromPoint(),
					booking.getToPoint());
		} else {
			if (!Common.isNullOrEmpty(booking.AddrFrom)
					&& !Common.isNullOrEmpty(booking.AddrTo)) {
				txtDistance.setText(this
						.getString(R.string.map_calculating_dist));
				mapRoute.calculateRoute(booking.AddrFrom, booking.AddrTo);
			}
		}
	}

	public void updateDistance() {
		int distance = mapRoute.getDistance();
		txtDistance.setText(displayedDistance(distance));
		booking.ComputedDistance = distance;
		booking.TaxiId = 0;
		booking.SelectedTaxi = null;
		txtTaxi.setText(getString(R.string.select_taxi));
	}

	private String displayedDistance(int distance) {
		if (distance == 0) {
			return getString(R.string.distance);
		}
		if (distance < 1000) {
			return new StringBuilder().append(Math.ceil(distance)).append(" m")
					.toString();
		} else {
			return new StringBuilder().append(Math.ceil(distance) / 1000)
					.append(" km").toString();
		}
	}

	private static String pad(int c) {
		if (c >= 10)
			return String.valueOf(c);
		else
			return "0" + String.valueOf(c);
	}

	private void makeBooking() {
		Toast.makeText(BookActivity.this, "Booking created", Toast.LENGTH_SHORT)
				.show();
	}

	private void cancelBooking() {
		Toast.makeText(BookActivity.this, "Booking cancelled",
				Toast.LENGTH_SHORT).show();
	}

	@Override
	protected void onSaveInstanceState(Bundle savedInstance) {
		if (savedInstance != null) {
			savedInstance.putBoolean(Common.NEWBOOKING_FLAG, newBooking);
			Gson g = new Gson();
			savedInstance.putString(Common.BOOKING_FLAG, g.toJson(booking));
		}
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

	private OnClickListener pickTimeClickListener = new OnClickListener() {
		@SuppressWarnings("deprecation")
		public void onClick(View v) {
			showDialog(Common.TIME_DIALOG_ID);
		}
	};
	private OnClickListener pickDateClickListener = new OnClickListener() {
		@SuppressWarnings("deprecation")
		public void onClick(View v) {
			showDialog(Common.DATEPICKER_DIALOG_ID);
		}
	};
	private OnClickListener pickNumPeopleClickListener = new OnClickListener() {
		@SuppressWarnings("deprecation")
		public void onClick(View v) {
			showDialog(Common.NUMPEOPLE_DIALOG_ID);
		}
	};
	private OnClickListener btnDistanceClickListener = new OnClickListener() {
		public void onClick(View v) {
			Intent intent = new Intent(BookActivity.this, ShowMapActivity.class);
			Bundle b = new Bundle();
			Gson g = new Gson();
			b.putString(Common.BOOKING_FLAG, g.toJson(booking));
			b.putBoolean(Common.ADDRESS_LOCKED_FLAG, true);
			intent.putExtras(b);
			startActivity(intent);
		}
	};

	private OnClickListener btnTaxiListener = new OnClickListener() {
		public void onClick(View view) {
			Intent intent = new Intent(BookActivity.this, TaxiActivity.class);
			Bundle b = new Bundle();
			Gson g = new Gson();
			b.putString(Common.BOOKING_FLAG, g.toJson(booking));
			intent.putExtras(b);
			startActivityForResult(intent, Common.PICK_TAXI);
		}
	};

	private OnClickListener mMapFromListener = new OnClickListener() {
		public void onClick(View view) {
			Intent intent = new Intent(BookActivity.this, ShowMapActivity.class);
			Bundle b = new Bundle();
			Gson g = new Gson();
			b.putString(Common.BOOKING_FLAG, g.toJson(booking));
			b.putBoolean(Common.MAPFROM_FLAG, true);
			intent.putExtras(b);
			startActivityForResult(intent, Common.PICK_ADDRESS_FROM);
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
									booking.setFromPoint(null);
									booking.AddrFrom = inputFrom.getText()
											.toString();
									updateAddress();
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
			Gson g = new Gson();
			b.putString(Common.BOOKING_FLAG, g.toJson(booking));
			b.putBoolean(Common.MAPFROM_FLAG, false);
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
									booking.setToPoint(null);
									booking.AddrTo = inputTo.getText()
											.toString();
									updateAddress();
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
			if (requestCode == Common.PICK_ADDRESS_FROM
					|| requestCode == Common.PICK_ADDRESS_TO) {
				if (resultCode == RESULT_OK) {
					Gson g = new Gson();
					String json = data.getStringExtra(Common.BOOKING_FLAG);
					booking = g.fromJson(json, Entities.Booking.class);
					updateAddress();
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
					((TextView) findViewById(R.id.txtPrice)).setText(booking.getPriceEstimate());
				}
			}
		}
	}
}
