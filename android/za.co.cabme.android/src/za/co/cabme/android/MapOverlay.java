package za.co.cabme.android;

import java.io.IOException;
import java.lang.reflect.InvocationTargetException;
import java.lang.reflect.Method;
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
import android.util.Log;
import android.view.LayoutInflater;
import android.view.MotionEvent;
import android.view.View;
import android.widget.RelativeLayout;
import android.widget.TextView;

public class MapOverlay extends Overlay {
	Bitmap marker;
	GeoPoint p;
	Context context;
	BalloonLayout noteBalloon;
	boolean locked;
	MapView mapView;
	Method delegate;
	Object obj;
	Address addr;
	String firstAddr;

	public MapOverlay(Context context, Object obj, Method delegate) {
		this.context = context;
		this.locked = false;
		this.delegate = delegate;
		this.obj = obj;
		setupBalloon();
	}

	public MapOverlay(Context context, Object obj, Method delegate,
			MapView mapView, GeoPoint firstPoint, String lockedAddress,
			boolean locked) {
		this.context = context;
		this.delegate = delegate;
		this.obj = obj;
		this.locked = locked;
		this.mapView = mapView;
		firstAddr = lockedAddress;
		Log.i(MapOverlay.class.getName(), "loading:" + firstAddr + " at "
				+ firstPoint);
		setupBalloon();
		if (firstPoint != null) {
			p = firstPoint;
			drawBalloon(mapView, lockedAddress);
		} else {
			getFirstPoint();
		}
	}

	private void setupBalloon() {
		marker = BitmapFactory.decodeResource(context.getResources(),
				R.drawable.ic_launcher_point);

		LayoutInflater layoutInflater = (LayoutInflater) context
				.getSystemService(Context.LAYOUT_INFLATER_SERVICE);
		noteBalloon = (BalloonLayout) layoutInflater.inflate(
				R.layout.balloonlayout, null);
		RelativeLayout.LayoutParams layoutParams = new RelativeLayout.LayoutParams(
				200, 100);
		layoutParams.addRule(RelativeLayout.CENTER_VERTICAL);
		layoutParams.addRule(RelativeLayout.CENTER_HORIZONTAL);
		noteBalloon.setLayoutParams(layoutParams);
	}

	private void getFirstPoint() {
		new AsyncTask<String, Void, GeoPoint>() {
			@Override
			protected GeoPoint doInBackground(String... address) {
				try {
					Geocoder geoCoder = new Geocoder(context,
							Locale.getDefault());
					Address addr = geoCoder.getFromLocationName(address[0], 1)
							.get(0);
					return new GeoPoint((int) (addr.getLatitude() * 1e6),
							(int) (addr.getLongitude() * 1e6));
				} catch (IOException ex) {
					ex.printStackTrace();
				}
				return null;
			}

			@Override
			protected void onPostExecute(GeoPoint point) {
				p = point;
				drawBalloon(mapView, firstAddr);
			}
		}.execute(firstAddr);
	}

	public GeoPoint getPoint() {
		return p;
	}

	public String getAddressString() {
		String add = "";
		if (addr != null) {
			for (int i = 0; i < addr.getMaxAddressLineIndex(); i++) {
				add += addr.getAddressLine(i) + ",\n";
			}
			if (add.endsWith(",\n")) {
				add = add.substring(0, add.length() - 2);
			}
		}
		if (Common.isNullOrEmpty(add)
				|| add.equals(context.getString(R.string.map_loading_addr))) {
			return null;
		}
		return add;
	}

	@Override
	public boolean draw(Canvas canvas, MapView mapView, boolean shadow,
			long when) {
		super.draw(canvas, mapView, shadow);

		if (p != null) {
			Point screenPts = new Point();
			mapView.getProjection().toPixels(p, screenPts);

			Paint panelPaint = new Paint();
			panelPaint.setAntiAlias(true);
			panelPaint.setARGB(230, 255, 216, 0);

			int r = 15, offset = 30;

			Path baloonTip = new Path();
			baloonTip.moveTo(screenPts.x - r, screenPts.y - offset);
			baloonTip.lineTo(screenPts.x, screenPts.y);
			baloonTip.lineTo(screenPts.x + r, screenPts.y - offset);
			canvas.drawPath(baloonTip, panelPaint);

			canvas.drawArc(new RectF(screenPts.x - r, screenPts.y - offset - r,
					screenPts.x + r, screenPts.y - offset + r), 180, 180, true,
					panelPaint);

			panelPaint.setARGB(230, 0, 0, 0);
			canvas.drawCircle(screenPts.x, screenPts.y - offset , r / 3,
					panelPaint);

			// canvas.drawBitmap(marker, screenPts.x - (marker.getWidth() / 2),
			// screenPts.y - (marker.getHeight()), null);
		}
		return true;
	}

	private void drawBalloon(MapView mapView, String loadText) {
		if (noteBalloon != null) {
			mapView.removeView(noteBalloon);
			noteBalloon.setVisibility(View.VISIBLE);
			((TextView) noteBalloon.findViewById(R.id.note_text))
					.setText(loadText);
			mapView.addView(noteBalloon, new MapView.LayoutParams(200, 200, p,
					MapView.LayoutParams.BOTTOM_CENTER));
		}

		new AsyncTask<GeoPoint, Void, Address>() {
			@Override
			protected Address doInBackground(GeoPoint... geoPoints) {
				try {
					addr = null;
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
				addr = address;
				if (address != null) {
					if (noteBalloon != null) {
						((TextView) noteBalloon.findViewById(R.id.note_text))
								.setText(getAddressString());
					}
					if (delegate != null) {
						try {
							delegate.invoke(obj, new Object[0]);
						} catch (IllegalArgumentException e) {
							e.printStackTrace();
						} catch (IllegalAccessException e) {
							e.printStackTrace();
						} catch (InvocationTargetException e) {
							e.printStackTrace();
						}
					}
				}
			}
		}.execute(p);
	}

	@Override
	public boolean onTap(GeoPoint p, MapView mapView) {
		if (!locked) {
			this.p = p;
			if (noteBalloon != null) {
				drawBalloon(mapView,
						context.getString(R.string.map_loading_addr));
			}
		}
		return true;
	}

	@Override
	public boolean onTouchEvent(MotionEvent event, MapView mapView) {
		mapView.invalidate();
		return false;
	}
}
