package za.co.cabme.android;

import android.content.Context;
import android.view.*;
import android.widget.*;

public class BookingAdapter extends ArrayAdapter<String> {
	private final Context context;
	private final String[] values;

	public BookingAdapter(Context context, String[] values) {
		super(context, R.layout.rowlayout, values);
		this.context = context;
		this.values = values;
	}

	@Override
	public View getView(int position, View convertView, ViewGroup parent) {
		LayoutInflater inflater = (LayoutInflater) context
				.getSystemService(Context.LAYOUT_INFLATER_SERVICE);
		View rowView = inflater.inflate(R.layout.rowlayout, parent, false);
		TextView textView = (TextView) rowView.findViewById(R.id.label);		
		// Change the icon for Windows and iPhone
		String s = values[position];
		if (s.startsWith("Windows7") || s.startsWith("iPhone")
				|| s.startsWith("Solaris")) {
			textView.setText(s + " Test");
		} else {
			textView.setText(s);
		}

		return rowView;
	}
}
