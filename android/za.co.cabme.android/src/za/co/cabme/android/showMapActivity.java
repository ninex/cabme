package za.co.cabme.android;

import android.app.ActionBar;
import android.content.Context;
import android.content.Intent;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.location.Location;
import android.location.LocationListener;
import android.location.LocationManager;
import android.os.Bundle;
import android.view.Menu;
import android.view.MenuInflater;
import android.view.MenuItem;
import android.widget.TextView;

import com.google.android.maps.GeoPoint;
import com.google.android.maps.MapActivity;
import com.google.android.maps.MapController;
import com.google.android.maps.MapView;
import com.google.android.maps.MyLocationOverlay;

public class showMapActivity extends MapActivity {

	public static final String KEY_ADDRESS = "za.co.cabme.android.Address";

	private MapController mapController;
	private MapView mapView;
	private LocationManager locationManager;
	private mapOverlay selectOverlay;
	private MyLocationOverlay myLocationOverlay;
	private GeoUpdateHandler geoListener;

	public void onCreate(Bundle bundle) {
		super.onCreate(bundle);
		setContentView(R.layout.maplayout); // bind the layout to the activity
		getActionBar().setDisplayOptions(
				ActionBar.DISPLAY_SHOW_HOME | ActionBar.DISPLAY_HOME_AS_UP
						| ActionBar.DISPLAY_SHOW_TITLE);
		TextView txtAddress = (TextView) findViewById(R.id.txtAddress);
		// Configure the Map
		mapView = (MapView) findViewById(R.id.mapview);
		mapView.setBuiltInZoomControls(true);
		mapView.setSatellite(false);
		// configure the controller
		mapController = mapView.getController();
		mapController.setZoom(17); // Zoom 1 is world view
		// Configure location manager
		geoListener = new GeoUpdateHandler();
		locationManager = (LocationManager) getSystemService(Context.LOCATION_SERVICE);
		locationManager.requestLocationUpdates(LocationManager.GPS_PROVIDER,
				100, 10, geoListener);

		// Point select overlay
		Bitmap bmp = BitmapFactory.decodeResource(this.getResources(),
				R.drawable.ic_launcher_point);
		selectOverlay = new mapOverlay(getBaseContext(), bmp, txtAddress);
		mapView.getOverlays().add(selectOverlay);
		// Add my location overlay
		myLocationOverlay = new MyLocationOverlay(this, mapView);
		mapView.getOverlays().add(myLocationOverlay);
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
			i.putExtra(KEY_ADDRESS,
					((TextView) findViewById(R.id.txtAddress)).getText());
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
		myLocationOverlay.enableMyLocation();
		myLocationOverlay.enableCompass();
	}

	@Override
	protected void onPause() {
		super.onPause();
		myLocationOverlay.disableMyLocation();
		myLocationOverlay.disableCompass();
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
