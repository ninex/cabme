package za.co.cabme.android;

import com.google.gson.Gson;

import android.app.ActionBar;
import android.app.ListActivity;
import android.app.ProgressDialog;
import android.content.Intent;
import android.os.AsyncTask;
import android.os.Bundle;
import android.view.MenuItem;
import android.view.View;
import android.widget.ListView;

public class TaxiActivity extends ListActivity {

	private ProgressDialog pd;
	private Entities.Booking booking;

	public void onCreate(Bundle state) {
		super.onCreate(state);
		getActionBar().setDisplayOptions(
				ActionBar.DISPLAY_SHOW_HOME | ActionBar.DISPLAY_HOME_AS_UP
						| ActionBar.DISPLAY_SHOW_TITLE);

		Bundle b = getIntent().getExtras();
		if (b != null) {
			Gson g = new Gson();
			String json = b.getString(Common.BOOKING_FLAG);
			booking = g.fromJson(json, Entities.Booking.class);
		}
		getActionBar().setTitle("Taxis");

		t.execute(booking);

		pd = ProgressDialog.show(this, "Loading...", "Retrieving taxi info.",
				false);

	}

	@Override
	public boolean onOptionsItemSelected(MenuItem item) {
		switch (item.getItemId()) {
		case android.R.id.home:
			Intent i = new Intent();
			setResult(RESULT_CANCELED, i);
			finish();
			return true;
		default:
			return super.onOptionsItemSelected(item);
		}
	}

	@Override
	protected void onListItemClick(ListView l, View v, int position, long id) {
		Entities.Taxi item = (Entities.Taxi) getListAdapter().getItem(position);
		Intent i = new Intent();
		Gson g = new Gson();
		String json = g.toJson(item);
		i.putExtra(Common.TAXI_FLAG, json);
		setResult(RESULT_OK, i);
		finish();
	}

	private AsyncTask<Entities.Booking, Void, Entities.Taxi[]> t = new AsyncTask<Entities.Booking, Void, Entities.Taxi[]>() {
		@Override
		protected Entities.Taxi[] doInBackground(Entities.Booking... arg0) {
			String json = Common.queryRESTurl(getString(R.string.baseWebUrl)
					+ "/taxis");
			Gson gson = new Gson();
			Entities.Taxi[] taxis = gson.fromJson(json, Entities.Taxi[].class);
			return taxis;
		}

		@Override
		protected void onPostExecute(Entities.Taxi[] list) {
			if (list != null) {
				TaxiAdapter adapter = new TaxiAdapter(getBaseContext(), list, booking);
				setListAdapter(adapter);
			}
			pd.dismiss();
		}
	};
}
