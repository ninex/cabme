package za.co.cabme.android;

import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.TextView;

public class TaxiAdapter extends ArrayAdapter<Entities.Taxi> {
	private final Context context;
	private final Entities.Taxi[] values;
	private final Entities.Booking booking;

	public TaxiAdapter(Context context, Entities.Taxi[] values, Entities.Booking booking) {
		super(context, R.layout.taxirow, values);
		this.context = context;
		this.values = values;
		this.booking = booking;
	}

	@Override
	public View getView(int position, View convertView, ViewGroup parent) {
		LayoutInflater inflater = (LayoutInflater) context
				.getSystemService(Context.LAYOUT_INFLATER_SERVICE);
		View rowView = inflater.inflate(R.layout.taxirow, parent, false);
		TextView label = (TextView) rowView.findViewById(R.id.label);	
		TextView price = (TextView) rowView.findViewById(R.id.price);		
		
		Entities.Taxi taxi = values[position];
		label.setText(taxi.Name);
		booking.SelectedTaxi = taxi;
		price.setText(booking.getPriceEstimate());

		return rowView;
	}
}
