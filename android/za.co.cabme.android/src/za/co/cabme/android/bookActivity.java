package za.co.cabme.android;

import android.app.Activity;
import android.os.Bundle;
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
		Bundle b = getIntent().getExtras();
		if (b!= null){
			newBooking = b.getBoolean(NEWBOOKING_FLAG);
			if (newBooking){
		        Button btnBook = (Button) findViewById(R.id.btnBook);
		        btnBook.setOnClickListener(mBookListener);
		    }else{
		        Button btnBook = (Button) findViewById(R.id.btnBook);
		        btnBook.setOnClickListener(mBookListener);
		    }
		}
	}

	private OnClickListener mBookListener = new OnClickListener() {
		public void onClick(View view) {
			Toast.makeText(bookActivity.this, "Creating booking",
					Toast.LENGTH_SHORT).show();
			finish();
		}
	};
}
