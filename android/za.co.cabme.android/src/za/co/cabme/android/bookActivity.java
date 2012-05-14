package za.co.cabme.android;

import android.app.Activity;
import android.os.Bundle;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.Toast;

public class bookActivity extends Activity {
    /** Called when the activity is first created. */
    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.book);
        Button btnBook = (Button)findViewById(R.id.btnBook);
        btnBook.setOnClickListener(mBookListener);
    }
    
    private OnClickListener mBookListener = new OnClickListener(){    
	    public void onClick(View view){
	    	Toast.makeText(bookActivity.this, "Creating booking", Toast.LENGTH_SHORT).show();
	    	finish();
	    }
    };
}
