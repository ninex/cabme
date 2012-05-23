package za.co.cabme.android;

import android.content.Context;
import android.view.*;
import android.widget.*;

public class BookingAdapter extends ArrayAdapter<Entities.Booking> {
	private final Context context;
	private final Entities.Booking[] values;

	public BookingAdapter(Context context, Entities.Booking[] values) {
		super(context, R.layout.rowlayout, values);
		this.context = context;
		this.values = values;
	}

	@Override
	public View getView(int position, View convertView, ViewGroup parent) {
		LayoutInflater inflater = (LayoutInflater) context
				.getSystemService(Context.LAYOUT_INFLATER_SERVICE);
		View rowView = inflater.inflate(R.layout.rowlayout, parent, false);
		TextView label = (TextView) rowView.findViewById(R.id.label);	
		Entities.Booking booking = values[position];
		
		label.setText(booking.AddrTo);

		return rowView;
	}
}
