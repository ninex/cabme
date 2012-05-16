package za.co.cabme.android;

import android.app.ActionBar;
import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.view.MenuItem;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.Toast;

public class bookActivity extends Activity {
	public static final String NEWBOOKING_FLAG = "NEWBOOKING_FLAG";
	boolean newBooking = false;
	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.book);
		getActionBar().setDisplayOptions(ActionBar.DISPLAY_SHOW_HOME | ActionBar.DISPLAY_HOME_AS_UP | ActionBar.DISPLAY_SHOW_TITLE);
		Bundle b = getIntent().getExtras();
		if (b!= null){
			newBooking = b.getBoolean(NEWBOOKING_FLAG);
			if (newBooking){
		        Button btnBook = (Button) findViewById(R.id.btnBook);
		        btnBook.setOnClickListener(mBookListener);
		        getActionBar().setTitle("Make Booking");
		    }else{
		        Button btnBook = (Button) findViewById(R.id.btnBook);
		        btnBook.setOnClickListener(mBookListener);
		        getActionBar().setTitle("View Booking");
		    }
		}
		Button btnBook = (Button) findViewById(R.id.btnFrom);
        btnBook.setOnClickListener(mMapListener);
	}
	
	@Override
	public boolean onOptionsItemSelected(MenuItem item) {
	    switch (item.getItemId()) {
	        case android.R.id.home:
	            // app icon in action bar clicked; go home
	            Intent intent = new Intent(this, cabmeActivity.class);
	            intent.addFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP);
	            startActivity(intent);
	            return true;
	        default:
	            return super.onOptionsItemSelected(item);
	    }
	}
	


	private OnClickListener mMapListener = new OnClickListener() {
		public void onClick(View view) {
			Intent intent = new Intent(bookActivity.this, showMapActivity.class);
			startActivity(intent);
		}
	};

	private OnClickListener mBookListener = new OnClickListener() {
		public void onClick(View view) {
			Toast.makeText(bookActivity.this, "Booking created",
					Toast.LENGTH_SHORT).show();
			finish();
		}
	};
}
