package za.co.cabme.android;

import java.io.IOException;
import java.util.List;
import java.util.Locale;

import com.google.android.maps.GeoPoint;
import com.google.android.maps.MapView;
import com.google.android.maps.Overlay;

import android.content.Context;
import android.graphics.*;
import android.location.Address;
import android.location.Geocoder;
import android.os.AsyncTask;
import android.view.MotionEvent;
import android.view.View;
import android.widget.TextView;

public class MapOverlay extends Overlay {
	Bitmap marker;
	GeoPoint p;
	Context context;
	BalloonLayout noteBalloon;

	public MapOverlay(Context context, Bitmap marker, BalloonLayout noteBalloon) {
		this.marker = marker;
		this.context = context;
		this.noteBalloon = noteBalloon;
	}

	public GeoPoint getPoint() {
		return p;
	}

	@Override
	public boolean draw(Canvas canvas, MapView mapView, boolean shadow,
			long when) {
		super.draw(canvas, mapView, shadow);

		if (p != null) {
			Point screenPts = new Point();
			mapView.getProjection().toPixels(p, screenPts);

			canvas.drawBitmap(marker,
					screenPts.x - (marker.getWidth() / 2), screenPts.y
							- (marker.getHeight() ), null);
		}
		return true;
	}

	@Override
	public boolean onTap(GeoPoint p, MapView mapView) {
		this.p = p;
		if (noteBalloon != null) {
			mapView.removeView(noteBalloon);
			noteBalloon.setVisibility(View.VISIBLE);
			((TextView) noteBalloon.findViewById(R.id.note_text))
					.setText(R.string.map_loading_addr);
			mapView.addView(noteBalloon, new MapView.LayoutParams(200, 200, p,
					MapView.LayoutParams.BOTTOM_CENTER));
		}

		new AsyncTask<GeoPoint, Void, Address>() {
			@Override
			protected Address doInBackground(GeoPoint... geoPoints) {
				try {
					Geocoder geoCoder = new Geocoder(context,
							Locale.getDefault());
					double latitude = geoPoints[0].getLatitudeE6() / 1E6;
					double longitude = geoPoints[0].getLongitudeE6() / 1E6;
					List<Address> addresses = geoCoder.getFromLocation(
							latitude, longitude, 1);
					if (addresses.size() > 0)
						return addresses.get(0);
				} catch (IOException ex) {
					ex.printStackTrace();
				}
				return null;
			}

			@Override
			protected void onPostExecute(Address address) {
				if (address != null) {
					String add = "";
					for (int i = 0; i < address.getMaxAddressLineIndex(); i++) {
						add += address.getAddressLine(i) + ",\n";
					}					
						if (add.endsWith(",\n")) {
							add = add.substring(0, add.length() - 2);
						}
					if (noteBalloon != null) {
						((TextView) noteBalloon.findViewById(R.id.note_text))
								.setText(add);
					}
				}
			}
		}.execute(p);
		return true;
	}

	@Override
	public boolean onTouchEvent(MotionEvent event, MapView mapView) {
		mapView.invalidate();
		return false;
	}
}
