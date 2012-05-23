package za.co.cabme.android;

import android.app.ActionBar;
import android.app.ListActivity;
import android.content.Intent;
import android.os.Bundle;
import android.view.*;
import android.widget.*;

public class BookingListActivity extends ListActivity {
	boolean review = false;

	public void onCreate(Bundle icicle) {
		super.onCreate(icicle);
		getActionBar().setDisplayOptions(ActionBar.DISPLAY_SHOW_HOME | ActionBar.DISPLAY_HOME_AS_UP | ActionBar.DISPLAY_SHOW_TITLE);
                
		Bundle b = getIntent().getExtras();
		if (b!= null){
			review = b.getBoolean(Common.REVIEW_FLAG);
		}
		if (review){
			getActionBar().setTitle("Review Booking");
		}else{
			getActionBar().setTitle("Bookings");
		}
		
		String[] values = new String[] { "Android", "iPhone", "WindowsMobile",
				"Blackberry", "WebOS", "Ubuntu", "Windows7", "Max OS X",
				"Linux", "OS/2" };
		BookingAdapter adapter = new BookingAdapter(this, values);
		setListAdapter(adapter);

	}
	
	@Override
	public boolean onOptionsItemSelected(MenuItem item) {
	    switch (item.getItemId()) {
	        case android.R.id.home:
	            // app icon in action bar clicked; go home
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
		String item = (String) getListAdapter().getItem(position);		
		if (review) {
			Toast.makeText(this, item + " to be reviewed", Toast.LENGTH_LONG)
					.show();
		} else {
			Toast.makeText(this, item + " selected", Toast.LENGTH_LONG).show();
			Intent intent = new Intent(BookingListActivity.this, BookActivity.class);
	    	Bundle b = new Bundle();
	    	b.putBoolean(Common.NEWBOOKING_FLAG, false);
	    	intent.putExtras(b);
	    	startActivity(intent);
		}
	}
}
