package za.co.cabme.android;

import java.lang.reflect.Method;

import android.app.ActionBar;
import android.content.Context;
import android.content.Intent;
import android.location.Address;
import android.location.Location;
import android.location.LocationListener;
import android.location.LocationManager;
import android.os.Bundle;
import android.view.Menu;
import android.view.MenuInflater;
import android.view.MenuItem;

import com.google.android.maps.GeoPoint;
import com.google.android.maps.MapActivity;
import com.google.android.maps.MapController;
import com.google.android.maps.MapView;
import com.google.android.maps.MyLocationOverlay;

public class ShowMapActivity extends MapActivity {

	public static final String ADDRESS_LOCKED_FLAG = "za.co.cabme.android.AddressLockedFlag";
	public static final String FROMADDR_FLAG = "za.co.cabme.android.FromAddressFlag";
	public static final String TOADDR_FLAG = "za.co.cabme.android.ToAddressFlag";
	public static final String FROMLAT_FLAG = "za.co.cabme.android.FromLatFlag";
	public static final String FROMLONG_FLAG = "za.co.cabme.android.FromLongFlag";
	public static final String TOLAT_FLAG = "za.co.cabme.android.ToLatFlag";
	public static final String TOLONG_FLAG = "za.co.cabme.android.ToLongFlag";

	private MapController mapController;
	private MapView mapView;
	private LocationManager locationManager;
	private MyLocationOverlay myLocationOverlay;
	private MapOverlay overlayFrom, overlayTo;
	private GeoUpdateHandler geoListener;
	private RouteOverlay routeOverlay;
	private MapRoute mapRoute;
	private boolean locked = false;

	public void onCreate(Bundle bundle) {
		super.onCreate(bundle);
		setContentView(R.layout.maplayout); // bind the layout to the activity
		getActionBar().setDisplayOptions(
				ActionBar.DISPLAY_SHOW_HOME | ActionBar.DISPLAY_HOME_AS_UP
						| ActionBar.DISPLAY_SHOW_TITLE);
		// Map view and GPS
		loadMapViewandGPS();
		// Routing
		loadRouting();
		// Delegate
		Method delegate = null;
		try {
			delegate = ShowMapActivity.class.getMethod("updateRoute",
					new Class[0]);
		} catch (NoSuchMethodException e) {
			e.printStackTrace();
		}
		routeOverlay = new RouteOverlay(getBaseContext(), null);
		mapView.getOverlays().add(routeOverlay);
		Bundle b = getIntent().getExtras();
		if (b != null) {
			String fromAddr = b.getString(FROMADDR_FLAG);
			String toAddr = b.getString(TOADDR_FLAG);

			int fromLat = b.getInt(FROMLAT_FLAG, 0);
			int fromLong = b.getInt(FROMLONG_FLAG, 0);
			int toLat = b.getInt(TOLAT_FLAG, 0);
			int toLong = b.getInt(TOLONG_FLAG, 0);
			GeoPoint from = null, to = null;
			if (fromLat != 0 && fromLong != 0) {
				from = new GeoPoint(fromLat, fromLong);
			}
			if (toLat != 0 && toLong != 0) {
				to = new GeoPoint(toLat, toLong);
			}
			locked = b.getBoolean(ADDRESS_LOCKED_FLAG, false);
			if (fromAddr == null || fromAddr.equals("")) {
				// Add my location overlay
				loadMyLocation();
				// Point select overlay
				overlayFrom = new MapOverlay(getBaseContext(), this, delegate);
				mapView.getOverlays().add(overlayFrom);
			} else {
				// Point select overlay
				overlayFrom = new MapOverlay(getBaseContext(), this, delegate,
						mapView, fromAddr, locked, from);
				mapView.getOverlays().add(overlayFrom);
				if (toAddr != null && !toAddr.equals("")) {
					// Point select overlay
					overlayTo = new MapOverlay(getBaseContext(), this,
							delegate, mapView, toAddr, locked, to);
					mapView.getOverlays().add(overlayTo);
				} else {
					// Point select overlay
					overlayTo = new MapOverlay(getBaseContext(), this, delegate);
					mapView.getOverlays().add(overlayTo);
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

	private void loadRouting() {
		Method delegate;
		try {
			delegate = ShowMapActivity.class.getMethod("redrawRoute",
					new Class[0]);
			mapRoute = new MapRoute(getBaseContext(), this, delegate);
		} catch (NoSuchMethodException e) {
			e.printStackTrace();
		}
	}

	public void updateRoute() {
		if (overlayFrom != null && overlayTo != null) {
			Address addrFrom = overlayFrom.getAddress();
			Address addrTo = overlayTo.getAddress();
			if (addrFrom != null && addrTo != null) {
				route(addrFrom, addrTo);
			}
		}
	}

	private void route(Address from, Address to) {
		mapRoute.calculateRoute(from, to);
	}

	public void redrawRoute() {
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
				if (overlayFrom != null) {
					i.putExtra(FROMADDR_FLAG, overlayFrom.getAddressString());
					GeoPoint p1 = overlayFrom.getPoint();
					if (p1 != null) {
						i.putExtra(FROMLAT_FLAG, p1.getLatitudeE6());
						i.putExtra(FROMLONG_FLAG, p1.getLongitudeE6());
					}
				}
				if (overlayTo != null) {
					i.putExtra(TOADDR_FLAG, overlayTo.getAddressString());
					GeoPoint p2 = overlayTo.getPoint();
					if (p2 != null) {
						i.putExtra(TOLAT_FLAG, p2.getLatitudeE6());
						i.putExtra(TOLONG_FLAG, p2.getLongitudeE6());
					}
				}
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
