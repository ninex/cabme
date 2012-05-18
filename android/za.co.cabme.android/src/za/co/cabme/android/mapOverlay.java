package za.co.cabme.android;

import java.io.IOException;
import java.util.List;
import java.util.Locale;

import com.google.android.maps.GeoPoint;
import com.google.android.maps.MapView;
import com.google.android.maps.Overlay;

import android.content.Context;
import android.graphics.Bitmap;
import android.graphics.Canvas;
import android.graphics.Point;
import android.location.Address;
import android.location.Geocoder;
import android.view.MotionEvent;
import android.widget.TextView;

public class mapOverlay extends Overlay {
	Bitmap drawable;
	GeoPoint p;
	Context context;
	TextView txtAddress;

	public mapOverlay(Context context, Bitmap drawable, TextView txtAddress) {
		this.drawable = drawable;
		this.context = context;
		this.txtAddress = txtAddress;
	}
	public GeoPoint getPoint(){
		return p;
	}
	@Override
	public boolean draw(Canvas canvas, MapView mapView, boolean shadow,
			long when) {
		super.draw(canvas, mapView, shadow);

		if (p != null) {
			// ---translate the GeoPoint to screen pixels---
			Point screenPts = new Point();
			mapView.getProjection().toPixels(p, screenPts);

			canvas.drawBitmap(drawable, screenPts.x, screenPts.y - 50, null);
		}
		return true;
	}

	@Override
	public boolean onTap(GeoPoint p, MapView mapView) {
		this.p = p;
		Geocoder geoCoder = new Geocoder(context, Locale.getDefault());
		try {
			List<Address> addresses = geoCoder.getFromLocation(
					p.getLatitudeE6() / 1E6, p.getLongitudeE6() / 1E6, 1);

			String add = "";
			if (addresses.size() > 0) {
				for (int i = 0; i < addresses.get(0).getMaxAddressLineIndex(); i++)
					add += addresses.get(0).getAddressLine(i) + ",\n";
			}
			if (txtAddress != null) {
				if (add.endsWith(",\n")){
					add = add.substring(0, add.length()-2);
				}
				txtAddress.setText(add);
			}
		} catch (IOException e) {
			e.printStackTrace();
		}
		return true;
	}

	@Override
	public boolean onTouchEvent(MotionEvent event, MapView mapView) {
		mapView.invalidate();
		return false;
	}
}
