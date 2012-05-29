package za.co.cabme.android;

import java.lang.reflect.Method;

import android.app.ActionBar;
import android.content.Context;
import android.content.Intent;
import android.location.Location;
import android.location.LocationListener;
import android.location.LocationManager;
import android.os.Bundle;
import android.view.Menu;
import android.view.MenuInflater;
import android.view.MenuItem;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.Button;

import com.google.android.maps.GeoPoint;
import com.google.android.maps.MapActivity;
import com.google.android.maps.MapController;
import com.google.android.maps.MapView;
import com.google.android.maps.MyLocationOverlay;
import com.google.gson.Gson;

public class ShowMapActivity extends MapActivity {

	private MapController mapController;
	private MapView mapView;
	private LocationManager locationManager;
	private MyLocationOverlay myLocationOverlay;
	private MapOverlay mapOverlay;
	private GeoUpdateHandler geoListener;
	private RouteOverlay routeOverlay;
	private MapRoute mapRoute;
	private boolean locked = false, mapFrom = true;
	private Entities.Booking booking;

	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.maplayout); // bind the layout to the activity
		getActionBar().setDisplayOptions(
				ActionBar.DISPLAY_SHOW_HOME | ActionBar.DISPLAY_HOME_AS_UP
						| ActionBar.DISPLAY_SHOW_TITLE);
		// Map view and GPS
		loadMapViewandGPS();
		// Routing
		loadRouting();
		routeOverlay = new RouteOverlay(getBaseContext(), null);
		mapView.getOverlays().add(routeOverlay);
		Bundle b;
		if (savedInstanceState == null) {
			b = getIntent().getExtras();
		} else {
			b = savedInstanceState;
		}
		if (b != null) {
			mapFrom = b.getBoolean(Common.MAPFROM_FLAG, true);
			String json = b.getString(Common.BOOKING_FLAG);
			Gson g = new Gson();
			booking = g.fromJson(json, Entities.Booking.class);
			if (booking != null) {
				locked = b.getBoolean(Common.ADDRESS_LOCKED_FLAG, false);
				loadPoints(mapFrom);
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
		locationManager.requestLocationUpdates(LocationManager.GPS_PROVIDER, 0,
				10, geoListener);
		locationManager.requestLocationUpdates(
				LocationManager.NETWORK_PROVIDER, 0, 10, geoListener);
		// point to last GPS location
		Location l = locationManager
				.getLastKnownLocation(LocationManager.NETWORK_PROVIDER);
		if (l != null) {
			int lat = (int) (l.getLatitude() * 1E6);
			int lng = (int) (l.getLongitude() * 1E6);
			mapController.animateTo(new GeoPoint(lat, lng));
		}
	}

	private void loadMyLocation(GeoPoint from) {
		myLocationOverlay = new MyLocationOverlay(this, mapView);
		mapView.getOverlays().add(myLocationOverlay);
		if (from == null) {
			myLocationOverlay.runOnFirstFix(new Runnable() {
				public void run() {
					mapController.animateTo(myLocationOverlay.getMyLocation());
				}
			});
		} else {
			mapController.animateTo(from);
		}
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

	private void loadPoints(boolean mapFrom) {
		// Delegate
		Method delegate = null;
		try {
			delegate = ShowMapActivity.class.getMethod("updateRoute",
					new Class[0]);
		} catch (NoSuchMethodException e) {
			e.printStackTrace();
		}
		Button btnFrom = (Button) findViewById(R.id.btnFrom);
		btnFrom.setOnClickListener(mbtnFromListener);
		Button btnTo = (Button) findViewById(R.id.btnTo);
		btnTo.setOnClickListener(mbtnToListener);

		GeoPoint from = null, to = null;
		from = booking.getFromPoint();
		to = booking.getToPoint();
		loadMyLocation(from);
		mapOverlay = new MapOverlay(getBaseContext(), this, delegate, mapView,
				from, to, booking.AddrFrom, booking.AddrTo, locked, mapFrom);
		mapView.getOverlays().add(mapOverlay);
	}

	private OnClickListener mbtnFromListener = new OnClickListener() {
		public void onClick(View view) {
			mapOverlay.useFrom();
		}
	};
	private OnClickListener mbtnToListener = new OnClickListener() {
		public void onClick(View view) {
			mapOverlay.useTo();
		}
	};

	public void updateRoute() {
		if (mapOverlay != null) {
			GeoPoint from = mapOverlay.getFromPoint(), to = mapOverlay
					.getToPoint();
			booking.setFromPoint(from);
			booking.setToPoint(to);
			mapRoute.calculateRoute(from, to);
		}
	}

	public void redrawRoute() {
		booking.ComputedDistance = mapRoute.getDistance();
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
				Gson g = new Gson();
				if (mapOverlay != null) {
					booking.AddrFrom = mapOverlay.getAddressFromString();
					booking.setFromPoint(mapOverlay.getFromPoint());
					booking.AddrTo = mapOverlay.getAddressToString();
					booking.setToPoint(mapOverlay.getToPoint());
				}
				i.putExtra(Common.BOOKING_FLAG, g.toJson(booking));
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
	protected void onSaveInstanceState(Bundle savedInstance) {
		if (savedInstance != null) {
			savedInstance.putBoolean(Common.MAPFROM_FLAG, mapFrom);
			savedInstance.putBoolean(Common.ADDRESS_LOCKED_FLAG, locked);
			Gson g = new Gson();
			savedInstance.putString(Common.BOOKING_FLAG, g.toJson(booking));
		}
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
