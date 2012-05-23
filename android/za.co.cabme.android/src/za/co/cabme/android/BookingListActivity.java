package za.co.cabme.android;

import com.google.gson.Gson;

import android.app.ActionBar;
import android.app.ListActivity;
import android.app.ProgressDialog;
import android.content.Context;
import android.content.Intent;
import android.os.AsyncTask;
import android.os.Bundle;
import android.telephony.TelephonyManager;
import android.view.*;
import android.widget.*;

public class BookingListActivity extends ListActivity {
	boolean review = false;
	private ProgressDialog pd;
	private String mPhoneNumber;

	public void onCreate(Bundle icicle) {
		super.onCreate(icicle);
		getActionBar().setDisplayOptions(
				ActionBar.DISPLAY_SHOW_HOME | ActionBar.DISPLAY_HOME_AS_UP
						| ActionBar.DISPLAY_SHOW_TITLE);
		mPhoneNumber = ((TelephonyManager) getBaseContext().getSystemService(
				Context.TELEPHONY_SERVICE)).getLine1Number();
		Bundle b = getIntent().getExtras();
		if (b != null) {
			review = b.getBoolean(Common.REVIEW_FLAG);
		}
		if (review) {
			getActionBar().setTitle("Review Booking");
		} else {
			getActionBar().setTitle("Bookings");
		}

		tActiveBooking.execute(mPhoneNumber);

		pd = ProgressDialog.show(this, "Loading...", "Retrieving bookings.",
				false);
	}

	@Override
	public boolean onOptionsItemSelected(MenuItem item) {
		switch (item.getItemId()) {
		case android.R.id.home:
			Intent intent = new Intent(this, CabmeActivity.class);
			intent.addFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP);
			startActivity(intent);
			return true;
		default:
			return super.onOptionsItemSelected(item);
		}
	}

	@Override
	protected void onListItemClick(ListView l, View v, int position, long id) {
		Entities.Booking item = (Entities.Booking) getListAdapter().getItem(
				position);
		if (review) {
			Toast.makeText(this, item.AddrTo + " to be reviewed",
					Toast.LENGTH_LONG).show();
		} else {
			Intent intent = new Intent(BookingListActivity.this,
					BookActivity.class);
			Gson g = new Gson();
			String json = g.toJson(item);
			Bundle b = new Bundle();
			b.putBoolean(Common.NEWBOOKING_FLAG, false);
			b.putString(Common.BOOKING_FLAG, json);
			intent.putExtras(b);
			startActivity(intent);
		}
	}

	private AsyncTask<String, Void, Entities.Booking[]> tActiveBooking = new AsyncTask<String, Void, Entities.Booking[]>() {
		@Override
		protected Entities.Booking[] doInBackground(String... number) {
			String json = Common.queryRESTurl(getString(R.string.baseWebUrl)
					+ "bookings?number=" + number[0]);
			Gson gson = new Gson();
			Entities.Booking[] bookings = gson.fromJson(json,
					Entities.Booking[].class);
			return bookings;
		}

		@Override
		protected void onPostExecute(Entities.Booking[] list) {
			if (list != null) {
				BookingAdapter adapter = new BookingAdapter(getBaseContext(),
						list);
				setListAdapter(adapter);
			}
			pd.dismiss();
		}
	};
}
