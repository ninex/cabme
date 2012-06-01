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
		TextView fromRow = (TextView) rowView.findViewById(R.id.fromRow);
		TextView toRow = (TextView) rowView.findViewById(R.id.toRow);
		TextView price = (TextView) rowView.findViewById(R.id.price);
		TextView taxi = (TextView) rowView.findViewById(R.id.taxi);
		Entities.Booking booking = values[position];

		label.setText(booking.PickupTime);
		fromRow.setText(context.getString(R.string.from) + ": "
				+ booking.AddrFrom);
		toRow.setText(context.getString(R.string.to) + ": " + booking.AddrTo);
		price.setText(context.getString(R.string.price) + ": "
				+ booking.getPriceEstimate());
		if (booking.SelectedTaxi != null) {
			taxi.setText(booking.SelectedTaxi.Name);
		}
		return rowView;
	}
}
