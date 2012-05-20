package za.co.cabme.android;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.util.ArrayList;
import java.util.List;
import java.util.Locale;

import org.apache.http.HttpEntity;
import org.apache.http.HttpResponse;
import org.apache.http.StatusLine;
import org.apache.http.client.ClientProtocolException;
import org.apache.http.client.HttpClient;
import org.apache.http.client.methods.HttpGet;
import org.apache.http.impl.client.DefaultHttpClient;

import android.app.ActionBar;
import android.content.Context;
import android.content.Intent;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.graphics.Canvas;
import android.graphics.Color;
import android.graphics.Paint;
import android.graphics.Path;
import android.graphics.Point;
import android.location.Address;
import android.location.Geocoder;
import android.location.Location;
import android.location.LocationListener;
import android.location.LocationManager;
import android.os.AsyncTask;
import android.os.Bundle;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.Menu;
import android.view.MenuInflater;
import android.view.MenuItem;
import android.widget.RelativeLayout;
import android.widget.TextView;

import com.google.android.maps.GeoPoint;
import com.google.android.maps.MapActivity;
import com.google.android.maps.MapController;
import com.google.android.maps.MapView;
import com.google.android.maps.MyLocationOverlay;
import com.google.android.maps.Overlay;
import com.google.android.maps.Projection;

public class ShowMapActivity extends MapActivity {

	public static final String KEY_ADDRESS_FROM = "za.co.cabme.android.AddressFrom";
	public static final String KEY_ADDRESS_TO = "za.co.cabme.android.AddressTo";
	public static final String KEY_ADDRESS_LOCKED = "za.co.cabme.android.AddressLocked";
	public static final String FROMADDR_FLAG = "za.co.cabme.android.FromAddressFlag";
	public static final String TOADDR_FLAG = "za.co.cabme.android.ToAddressFlag";

	private MapController mapController;
	private MapView mapView;
	private LocationManager locationManager;
	private MyLocationOverlay myLocationOverlay;
	private GeoUpdateHandler geoListener;
	private BalloonLayout noteBalloonFrom, noteBalloonTo;
	private RouteOverlay routeOverlay;
	private boolean locked = false;

	public void onCreate(Bundle bundle) {
		super.onCreate(bundle);
		setContentView(R.layout.maplayout); // bind the layout to the activity
		getActionBar().setDisplayOptions(
				ActionBar.DISPLAY_SHOW_HOME | ActionBar.DISPLAY_HOME_AS_UP
						| ActionBar.DISPLAY_SHOW_TITLE);

		loadMapViewandGPS();
		// Balloon
		loadNoteBalloon();
		// Point bitmap
		Bitmap bmp = BitmapFactory.decodeResource(this.getResources(),
				R.drawable.ic_launcher_point);
		routeOverlay = new RouteOverlay(getBaseContext(), null);
		mapView.getOverlays().add(routeOverlay);
		Bundle b = getIntent().getExtras();
		if (b != null) {
			String fromAddr = b.getString(FROMADDR_FLAG);
			String toAddr = b.getString(TOADDR_FLAG);
			locked = b.getBoolean(KEY_ADDRESS_LOCKED, false);
			if (fromAddr == null || fromAddr.equals("")) {
				// Add my location overlay
				loadMyLocation();
				// Point select overlay
				mapView.getOverlays().add(
						new MapOverlay(getBaseContext(), bmp, noteBalloonFrom));
			} else {
				// Point select overlay
				mapView.getOverlays().add(
						new MapOverlay(getBaseContext(), bmp, noteBalloonFrom,
								fromAddr, mapView, locked));
				if (toAddr != null && !toAddr.equals("")) {
					// Point select overlay
					mapView.getOverlays().add(
							new MapOverlay(getBaseContext(), bmp,
									noteBalloonTo, toAddr, mapView, locked));
					route(fromAddr, toAddr);
				} else {
					// Point select overlay
					mapView.getOverlays()
							.add(new MapOverlay(getBaseContext(), bmp,
									noteBalloonTo));
				}
			}
		}
	}

	private void loadMapViewandGPS() {
		// Configure the Map
		mapView = (MapView) findViewById(R.id.mapview);
		mapView.setBuiltInZoomControls(true);
		mapView.setSatellite(false);
		// GPS
		// configure the controller
		mapController = mapView.getController();
		mapController.setZoom(17); // Zoom 1 is world view
		// Configure location manager
		geoListener = new GeoUpdateHandler();
		locationManager = (LocationManager) getSystemService(Context.LOCATION_SERVICE);
		locationManager.requestLocationUpdates(LocationManager.GPS_PROVIDER,
				100, 10, geoListener);
		locationManager.requestLocationUpdates(
				LocationManager.NETWORK_PROVIDER, 100, 10, geoListener);
		// point to last GPS location
		Location l = locationManager
				.getLastKnownLocation(LocationManager.NETWORK_PROVIDER);
		int lat = (int) (l.getLatitude() * 1E6);
		int lng = (int) (l.getLongitude() * 1E6);
		mapController.animateTo(new GeoPoint(lat, lng));
	}

	private void loadMyLocation() {
		myLocationOverlay = new MyLocationOverlay(this, mapView);
		mapView.getOverlays().add(myLocationOverlay);
		myLocationOverlay.runOnFirstFix(new Runnable() {
			public void run() {
				mapController.animateTo(myLocationOverlay.getMyLocation());
			}
		});
	}

	private void loadNoteBalloon() {
		LayoutInflater layoutInflater = (LayoutInflater) this
				.getSystemService(Context.LAYOUT_INFLATER_SERVICE);
		noteBalloonFrom = (BalloonLayout) layoutInflater.inflate(
				R.layout.balloonlayout, null);
		noteBalloonTo = (BalloonLayout) layoutInflater.inflate(
				R.layout.balloonlayout, null);
		RelativeLayout.LayoutParams layoutParams = new RelativeLayout.LayoutParams(
				200, 100);
		layoutParams.addRule(RelativeLayout.CENTER_VERTICAL);
		layoutParams.addRule(RelativeLayout.CENTER_HORIZONTAL);
		noteBalloonFrom.setLayoutParams(layoutParams);
		noteBalloonTo.setLayoutParams(layoutParams);
	}

	public String getUrl(double srcLat, double srcLong, double destLat,
			double destLong) {

		StringBuilder urlString = new StringBuilder();

		urlString.append("http://maps.google.com/maps?f=d&hl=en");
		urlString.append("&saddr=");
		urlString.append(Double.toString(srcLat));
		urlString.append(",");
		urlString.append(Double.toString(srcLong));
		urlString.append("&daddr=");// to
		urlString.append(Double.toString(destLat));
		urlString.append(",");
		urlString.append(Double.toString(destLong));
		urlString.append("&ie=UTF8&0&om=0&output=dragdir");

		return urlString.toString();
	}

	public String queryRESTurl(String url) {
		StringBuilder builder = new StringBuilder();
		HttpClient client = new DefaultHttpClient();
		HttpGet httpGet = new HttpGet(url);
		try {
			HttpResponse response = client.execute(httpGet);
			StatusLine statusLine = response.getStatusLine();
			int statusCode = statusLine.getStatusCode();
			if (statusCode == 200) {
				HttpEntity entity = response.getEntity();
				InputStream content = entity.getContent();
				BufferedReader reader = new BufferedReader(
						new InputStreamReader(content));
				String line;
				while ((line = reader.readLine()) != null) {
					builder.append(line);
				}
			} else {
				Log.e(BookActivity.class.toString(), "Failed to download file");
			}
		} catch (ClientProtocolException e) {
			e.printStackTrace();
		} catch (IOException e) {
			e.printStackTrace();
		}
		return builder.toString();
	}

	private void route(String from, String to) {
		String[] list = new String[2];
		list[0] = from;
		list[1] = to;

		new AsyncTask<String[], Void, List<GeoPoint>>() {
			@Override
			protected List<GeoPoint> doInBackground(String[]... addresses) {
				List<GeoPoint> poly = new ArrayList<GeoPoint>();
				try {
					Geocoder geoCoder = new Geocoder(getBaseContext(),
							Locale.getDefault());
					Address fromAddr = geoCoder.getFromLocationName(
							addresses[0][0], 1).get(0);
					Address toAddr = geoCoder.getFromLocationName(
							addresses[0][1], 1).get(0);

					String encoded = queryRESTurl(getUrl(
							fromAddr.getLatitude(), fromAddr.getLongitude(),
							toAddr.getLatitude(), toAddr.getLongitude()));
					// get only the encoded geopoints
					encoded = encoded.split("points:\"")[1].split("\",")[0];
					// replace two backslashes by one (some error from the
					// transmission)
					encoded = encoded.replace("\\\\", "\\");

					// decoding

					int index = 0, len = encoded.length();
					int lat = 0, lng = 0;

					while (index < len) {
						int b, shift = 0, result = 0;
						do {
							b = encoded.charAt(index++) - 63;
							result |= (b & 0x1f) << shift;
							shift += 5;
						} while (b >= 0x20);
						int dlat = ((result & 1) != 0 ? ~(result >> 1)
								: (result >> 1));
						lat += dlat;

						shift = 0;
						result = 0;
						do {

							b = encoded.charAt(index++) - 63;
							result |= (b & 0x1f) << shift;
							shift += 5;
						} while (b >= 0x20);
						int dlng = ((result & 1) != 0 ? ~(result >> 1)
								: (result >> 1));
						lng += dlng;

						GeoPoint p = new GeoPoint(
								(int) (((double) lat / 1E5) * 1E6),
								(int) (((double) lng / 1E5) * 1E6));
						poly.add(p);
					}

				} catch (IOException ex) {
					ex.printStackTrace();
				}
				return poly;
			}

			@Override
			protected void onPostExecute(List<GeoPoint> poly) {
				if (poly != null && poly.size() > 0 && routeOverlay != null) {
					routeOverlay.SetPoints(poly);
				}
			}
		}.execute(list);
	}

	public class RouteOverlay extends Overlay {
		List<GeoPoint> points;
		Context context;

		public RouteOverlay(Context context, List<GeoPoint> points) {
			this.context = context;
			this.points = points;
		}

		public void SetPoints(List<GeoPoint> points) {
			this.points = points;
		}

		@Override
		public boolean draw(Canvas canvas, MapView mapView, boolean shadow,
				long when) {
			super.draw(canvas, mapView, shadow);

			if (points != null && points.size() > 0) {
				Paint mPaint = new Paint();
				mPaint.setDither(true);
				mPaint.setColor(Color.RED);
				mPaint.setStyle(Paint.Style.FILL_AND_STROKE);
				mPaint.setStrokeJoin(Paint.Join.ROUND);
				mPaint.setStrokeCap(Paint.Cap.ROUND);
				mPaint.setStrokeWidth(4);
				for (int i = 0; i < points.size() - 1; i++) {
					GeoPoint gP1 = points.get(i);
					GeoPoint gP2 = points.get(i + 1);

					Point p1 = new Point();
					Point p2 = new Point();

					Path path = new Path();

					Projection projection = mapView.getProjection();
					projection.toPixels(gP1, p1);
					projection.toPixels(gP2, p2);

					path.moveTo(p2.x, p2.y);
					path.lineTo(p1.x, p1.y);

					canvas.drawPath(path, mPaint);
				}
			}
			return true;
		}
	}

	@Override
	public boolean onCreateOptionsMenu(Menu menu) {
		MenuInflater inflater = getMenuInflater();
		inflater.inflate(R.menu.mapmenu, menu);
		return true;
	}

	@Override
	public boolean onOptionsItemSelected(MenuItem item) {
		Intent i = new Intent();
		switch (item.getItemId()) {
		case android.R.id.home:
			setResult(RESULT_CANCELED, i);
			finish();
			return true;
		case R.id.menu_Save:
			if (!locked) {
				i.putExtra(KEY_ADDRESS_FROM, ((TextView) noteBalloonFrom
						.findViewById(R.id.note_text)).getText());
				i.putExtra(KEY_ADDRESS_TO, ((TextView) noteBalloonTo
						.findViewById(R.id.note_text)).getText());
			}
			setResult(RESULT_OK, i);
			finish();
			return true;
		case R.id.menu_Cancel:
			setResult(RESULT_CANCELED, i);
			finish();
			return true;
		default:
			return super.onOptionsItemSelected(item);
		}
	}

	@Override
	public void onDestroy() {
		super.onDestroy();
		if (myLocationOverlay != null) {
			myLocationOverlay.disableMyLocation();
			myLocationOverlay.disableCompass();
		}
		if (geoListener != null) {
			locationManager.removeUpdates(geoListener);
		}
	}

	@Override
	protected boolean isRouteDisplayed() {
		return false;
	}

	@Override
	protected void onResume() {
		super.onResume();
		if (myLocationOverlay != null) {
			myLocationOverlay.enableMyLocation();
			myLocationOverlay.enableCompass();
		}
	}

	@Override
	protected void onPause() {
		super.onPause();
		if (myLocationOverlay != null) {
			myLocationOverlay.disableMyLocation();
			myLocationOverlay.disableCompass();
		}
	}

	public class GeoUpdateHandler implements LocationListener {

		public void onLocationChanged(Location location) {
			int lat = (int) (location.getLatitude() * 1E6);
			int lng = (int) (location.getLongitude() * 1E6);
			GeoPoint point = new GeoPoint(lat, lng);
			mapController.animateTo(point);

		}

		public void onProviderDisabled(String provider) {
		}

		public void onProviderEnabled(String provider) {
		}

		public void onStatusChanged(String provider, int status, Bundle extras) {
		}
	}
}
