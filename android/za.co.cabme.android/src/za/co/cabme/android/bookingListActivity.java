package za.co.cabme.android;

import android.app.ListActivity;
import android.os.Bundle;
import android.view.*;
import android.widget.*;

public class bookingListActivity extends ListActivity {
	public static final String REVIEW_FLAG = "REVIEWFLAG";
	boolean review = false;

	public void onCreate(Bundle icicle) {
		super.onCreate(icicle);

		Bundle b = getIntent().getExtras();
		if (b!= null){
			review = b.getBoolean(REVIEW_FLAG);
		}

		String[] values = new String[] { "Android", "iPhone", "WindowsMobile",
				"Blackberry", "WebOS", "Ubuntu", "Windows7", "Max OS X",
				"Linux", "OS/2" };
		ArrayAdapter<String> adapter = new ArrayAdapter<String>(this,
				R.layout.rowlayout, R.id.label, values);
		setListAdapter(adapter);
	}

	@Override
	protected void onListItemClick(ListView l, View v, int position, long id) {
		String item = (String) getListAdapter().getItem(position);
		if (review) {
			Toast.makeText(this, item + " to be reviewed", Toast.LENGTH_LONG)
					.show();
		} else {
			Toast.makeText(this, item + " selected", Toast.LENGTH_LONG).show();
		}
	}
}
