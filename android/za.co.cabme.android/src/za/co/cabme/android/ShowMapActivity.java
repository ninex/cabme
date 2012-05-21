package za.co.cabme.android;

import java.lang.reflect.Method;

import android.app.ActionBar;
import android.content.Context;
import android.content.Intent;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.location.Location;
import android.location.LocationListener;
import android.location.LocationManager;
import android.os.Bundle;
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
	private MapRoute mapRoute;
	private boolean locked = false;

	public void onCreate(Bundle bundle) {
		super.onCreate(bundle);
		setContentView(R.layout.maplayout); // bind the layout to the activity
		getActionBar().setDisplayOptions(
				ActionBar.DISPLAY_SHOW_HOME | ActionBar.DISPLAY_HOME_AS_UP
						| ActionBar.DISPLAY_SHOW_TITLE);
		//Map view and GPS
		loadMapViewandGPS();
		//Routing
		loadRouting();
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
	
	private void loadRouting(){
		Method delegate;
		try {
			delegate = ShowMapActivity.class.getMethod("updateRoute", new Class[0]);
			mapRoute = new MapRoute(getBaseContext(),this, delegate);
		} catch (NoSuchMethodException e) {			
			e.printStackTrace();
		}
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

	private void route(String from, String to) {
		mapRoute.calculateRoute(from, to);
	}

	public void updateRoute(){
		routeOverlay.SetPoints(mapRoute.getPoints());
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
