package za.co.cabme.android;

import android.app.ActionBar;
import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.view.Menu;
import android.view.MenuInflater;
import android.view.MenuItem;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.EditText;
import android.widget.Toast;

public class bookActivity extends Activity {
	public static final String NEWBOOKING_FLAG = "NEWBOOKING_FLAG";
	public static final int PICK_ADDRESS_FROM = 11;
	public static final int PICK_ADDRESS_TO = 12;
	boolean newBooking = false;

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
		Button btnFrom = (Button) findViewById(R.id.btnFrom);
		btnFrom.setOnClickListener(mMapFromListener);
		Button btnTo = (Button) findViewById(R.id.btnTo);
		btnTo.setOnClickListener(mMapToListener);
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
			Intent intent = new Intent(this, cabmeActivity.class);
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
		Toast.makeText(bookActivity.this, "Booking created", Toast.LENGTH_SHORT)
				.show();
	}

	private void cancelBooking() {
		Toast.makeText(bookActivity.this, "Booking cancelled",
				Toast.LENGTH_SHORT).show();
	}

	private OnClickListener mMapFromListener = new OnClickListener() {
		public void onClick(View view) {
			Intent intent = new Intent(bookActivity.this, showMapActivity.class);
			startActivityForResult(intent, PICK_ADDRESS_FROM);
		}
	};

	private OnClickListener mMapToListener = new OnClickListener() {
		public void onClick(View view) {
			Intent intent = new Intent(bookActivity.this, showMapActivity.class);
			startActivityForResult(intent, PICK_ADDRESS_TO);
		}
	};

	protected void onActivityResult(int requestCode, int resultCode, Intent data) {
		if (newBooking) {
			if (requestCode == PICK_ADDRESS_FROM) {
				if (resultCode == RESULT_OK) {
					EditText txtFrom = (EditText) findViewById(R.id.txtAddressFrom);
					txtFrom.setText(data
							.getCharSequenceExtra(showMapActivity.KEY_ADDRESS));
				}
			}
			if (requestCode == PICK_ADDRESS_TO) {
				if (resultCode == RESULT_OK) {
					EditText txtTo = (EditText) findViewById(R.id.txtAddressTo);
					txtTo.setText(data
							.getCharSequenceExtra(showMapActivity.KEY_ADDRESS));
				}
			}
		}
	}
}
