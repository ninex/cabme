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
import android.view.LayoutInflater;
import android.view.MotionEvent;
import android.view.View;
import android.widget.RelativeLayout;
import android.widget.TextView;

public class MapOverlay extends Overlay {
	GeoPoint from, to;
	boolean useFrom = true, regenFrom = true, regenTo = true;
	Context context;
	BalloonLayout balloonFrom, balloonTo;
	boolean locked;
	MapView mapView;
	Method delegate;
	Object obj;
	Address addrFrom, addrTo;
	String firstAddrFrom, firstAddrTo;

	public MapOverlay(Context context, Object obj, Method delegate,
			MapView mapView, GeoPoint firstPointFrom, GeoPoint firstPointTo,
			String addrFrom, String addrTo, boolean locked, boolean mapFrom) {
		this.context = context;
		this.delegate = delegate;
		this.obj = obj;
		this.locked = locked;
		this.mapView = mapView;
		this.useFrom = mapFrom;
		firstAddrFrom = addrFrom;
		firstAddrTo = addrTo;

		setupBalloons();
		if (firstPointFrom != null) {
			from = firstPointFrom;
			to = firstPointTo;
			drawBalloon(mapView, addrFrom);
		} else {
			if (!Common.isNullOrEmpty(firstAddrFrom)) {
				getFirstPoint();
			}
		}
	}

	public void useFrom() {
		useFrom = true;
	}

	public void useTo() {
		useFrom = false;
	}

	private void setupBalloons() {

		LayoutInflater layoutInflater = (LayoutInflater) context
				.getSystemService(Context.LAYOUT_INFLATER_SERVICE);
		balloonFrom = (BalloonLayout) layoutInflater.inflate(
				R.layout.balloonlayout, null);
		balloonTo = (BalloonLayout) layoutInflater.inflate(
				R.layout.balloonlayout, null);
		RelativeLayout.LayoutParams layoutParams = new RelativeLayout.LayoutParams(
				200, 100);
		layoutParams.addRule(RelativeLayout.CENTER_VERTICAL);
		layoutParams.addRule(RelativeLayout.CENTER_HORIZONTAL);
		balloonFrom.setLayoutParams(layoutParams);
		balloonTo.setLayoutParams(layoutParams);
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
				if (useFrom) {
					from = point;
				} else {
					to = point;
				}
				drawBalloon(mapView, firstAddrFrom);
			}
		}.execute(firstAddrFrom);
	}

	public GeoPoint getFromPoint() {
		return from;
	}

	public GeoPoint getToPoint() {
		return to;
	}

	public String getAddressFromString() {
		String add = "";
		if (addrFrom != null) {
			for (int i = 0; i < addrFrom.getMaxAddressLineIndex(); i++) {
				add += addrFrom.getAddressLine(i) + ",\n";
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

	public String getAddressToString() {
		String add = "";
		if (addrTo != null) {
			for (int i = 0; i < addrTo.getMaxAddressLineIndex(); i++) {
				add += addrTo.getAddressLine(i) + ",\n";
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

	private void drawPoint(Canvas canvas, GeoPoint p) {
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
			canvas.drawCircle(screenPts.x, screenPts.y - offset, r / 3,
					panelPaint);

			// Draw outlines
			panelPaint.setStyle(Paint.Style.STROKE);

			baloonTip = new Path();
			baloonTip.moveTo(screenPts.x - r, screenPts.y - offset);
			baloonTip.lineTo(screenPts.x, screenPts.y);
			baloonTip.lineTo(screenPts.x + r, screenPts.y - offset);
			canvas.drawPath(baloonTip, panelPaint);
			canvas.drawArc(new RectF(screenPts.x - r, screenPts.y - offset - r,
					screenPts.x + r, screenPts.y - offset + r), 180, 180,
					false, panelPaint);
		}
	}

	@Override
	public boolean draw(Canvas canvas, MapView mapView, boolean shadow,
			long when) {
		super.draw(canvas, mapView, shadow);
		drawPoint(canvas, from);
		drawPoint(canvas, to);

		return true;
	}

	private void drawBalloon(MapView mapView, String loadText) {
		if (balloonFrom != null && from != null && regenFrom) {
			mapView.removeView(balloonFrom);
			balloonFrom.setVisibility(View.VISIBLE);
			((TextView) balloonFrom.findViewById(R.id.note_text))
					.setText(loadText);
			mapView.addView(balloonFrom, new MapView.LayoutParams(200, 200,
					from, MapView.LayoutParams.BOTTOM_CENTER));
			new AsyncTask<GeoPoint, Void, Address>() {
				@Override
				protected Address doInBackground(GeoPoint... geoPoints) {
					try {
						addrFrom = null;
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
					addrFrom = address;
					if (address != null) {
						if (balloonFrom != null) {
							((TextView) balloonFrom
									.findViewById(R.id.note_text))
									.setText(getAddressFromString());
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
			}.execute(from);

		}
		if (balloonTo != null && to != null && regenTo) {
			mapView.removeView(balloonTo);
			balloonTo.setVisibility(View.VISIBLE);
			((TextView) balloonTo.findViewById(R.id.note_text))
					.setText(loadText);
			mapView.addView(balloonTo, new MapView.LayoutParams(200, 200, to,
					MapView.LayoutParams.BOTTOM_CENTER));
			new AsyncTask<GeoPoint, Void, Address>() {
				@Override
				protected Address doInBackground(GeoPoint... geoPoints) {
					try {
						addrTo = null;
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

					addrTo = address;
					if (address != null) {
						if (balloonTo != null) {
							((TextView) balloonTo.findViewById(R.id.note_text))
									.setText(getAddressToString());
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
			}.execute(to);

		}
	}

	@Override
	public boolean onTap(GeoPoint p, MapView mapView) {
		if (!locked) {			
			if (useFrom) {
				regenFrom = true;
				regenTo = false;
				this.from = p;
				if (balloonFrom != null) {
					drawBalloon(mapView,
							context.getString(R.string.map_loading_addr));
				}
			} else {
				regenFrom = false;
				regenTo = true;
				this.to = p;
				if (balloonTo != null) {
					 drawBalloon(mapView,
					 context.getString(R.string.map_loading_addr));
				}
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
