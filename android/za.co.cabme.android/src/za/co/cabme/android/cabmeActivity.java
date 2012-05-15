package za.co.cabme.android;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.ImageButton;
import android.widget.Toast;

public class cabmeActivity extends Activity {
    /** Called when the activity is first created. */
    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.main);
        //click events
        ImageButton btnBook = (ImageButton)findViewById(R.id.btnBook);
        btnBook.setOnClickListener(mBookListener);
        ImageButton btnViewBookings = (ImageButton)findViewById(R.id.btnViewBookings);
        btnViewBookings.setOnClickListener(mViewBookingsListener);
        ImageButton btnReview = (ImageButton)findViewById(R.id.btnReview);
        btnReview.setOnClickListener(mReviewListener);
        ImageButton btnSettings = (ImageButton)findViewById(R.id.btnSettings);
        btnSettings.setOnClickListener(mSettingsListener);
    }
    
    private OnClickListener mBookListener = new OnClickListener(){    
	    public void onClick(View view){
	    	Toast.makeText(cabmeActivity.this, "Launching Book Activity", Toast.LENGTH_SHORT).show();
	    	Intent intent = new Intent(cabmeActivity.this, bookActivity.class);
	    	startActivity(intent);
	    }
    };
    private OnClickListener mViewBookingsListener = new OnClickListener(){    
	    public void onClick(View view){
	    	Toast.makeText(cabmeActivity.this, "Launching list of bookings Activity", Toast.LENGTH_SHORT).show();
	    	Intent intent = new Intent(cabmeActivity.this, bookingListActivity.class);
	    	Bundle b = new Bundle();
	    	b.putBoolean(bookingListActivity.REVIEW_FLAG, false);
	    	intent.putExtras(b);
	    	startActivity(intent);
	    }
    };
    private OnClickListener mReviewListener = new OnClickListener(){    
	    public void onClick(View view){
	    	Toast.makeText(cabmeActivity.this, "Launching booking reviews Activity", Toast.LENGTH_SHORT).show();
	    	Intent intent = new Intent(cabmeActivity.this, bookingListActivity.class);
	    	Bundle b = new Bundle();
	    	b.putBoolean(bookingListActivity.REVIEW_FLAG, true);
	    	intent.putExtras(b);
	    	startActivity(intent);
	    }
    };
    private OnClickListener mSettingsListener = new OnClickListener(){    
	    public void onClick(View view){
	    	Toast.makeText(cabmeActivity.this, "Launching settings Activity", Toast.LENGTH_SHORT).show();
	    }
    };
}