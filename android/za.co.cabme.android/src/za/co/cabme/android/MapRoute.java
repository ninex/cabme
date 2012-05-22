package za.co.cabme.android;

import java.io.IOException;
import java.lang.reflect.InvocationTargetException;
import java.lang.reflect.Method;
import java.util.ArrayList;
import java.util.List;
import java.util.Locale;

import android.content.Context;
import android.location.Address;
import android.location.Geocoder;
import android.location.Location;
import android.os.AsyncTask;

import com.google.android.maps.GeoPoint;

public class MapRoute {

	Context context;
	List<GeoPoint> points;
	Method delegate;
	Object obj;

	public MapRoute(Context context, Object obj, Method delegate) {
		this.context = context;
		this.delegate = delegate;
		this.obj = obj;
	}

	public void calculateRoute(GeoPoint from, GeoPoint to) {
		GeoPoint[] list = new GeoPoint[2];
		list[0] = from;
		list[1] = to;
		// clear the current list of points
		points = null;
		new AsyncTask<GeoPoint[], Void, List<GeoPoint>>() {
			@Override
			protected List<GeoPoint> doInBackground(GeoPoint[]... addresses) {
				List<GeoPoint> poly = new ArrayList<GeoPoint>();
				GeoPoint fromAddr = addresses[0][0];
				GeoPoint toAddr = addresses[0][1];

				String encoded = Common.queryRESTurl(getUrl(
						(double) fromAddr.getLatitudeE6() / 1e6,
						(double) fromAddr.getLongitudeE6() / 1e6,
						(double) toAddr.getLatitudeE6() / 1e6,
						(double) toAddr.getLongitudeE6() / 1e6));
				// get only the encoded GeoPoints
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
				return poly;
			}

			@Override
			protected void onPostExecute(List<GeoPoint> poly) {
				points = poly;
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
		}.execute(list);
	}

	public void calculateRoute(Address from, Address to) {
		GeoPoint pointFrom = new GeoPoint((int) (from.getLatitude() * 1E6),
				(int) (from.getLongitude() * 1E6));
		GeoPoint pointTo = new GeoPoint((int) (to.getLatitude() * 1E6),
				(int) (to.getLongitude() * 1E6));
		calculateRoute(pointFrom, pointTo);
	}

	public void calculateRoute(String from, String to) {
		String[] list = new String[2];
		list[0] = from;
		list[1] = to;
		// clear the current list of points
		points = null;
		new AsyncTask<String[], Void, List<Address>>() {
			@Override
			protected List<Address> doInBackground(String[]... addresses) {
				List<Address> list = new ArrayList<Address>();
				Geocoder geoCoder = new Geocoder(context, Locale.getDefault());
				Address fromAddr;
				try {
					fromAddr = geoCoder.getFromLocationName(addresses[0][0], 1)
							.get(0);

					Address toAddr = geoCoder.getFromLocationName(
							addresses[0][1], 1).get(0);
					list.add(fromAddr);
					list.add(toAddr);
				} catch (IOException e) {
					e.printStackTrace();
				}
				return list;
			}

			@Override
			protected void onPostExecute(List<Address> list) {
				if (list != null && list.size() >= 2) {
					calculateRoute(list.get(0), list.get(1));
				}
			}
		}.execute(list);
	}

	public float getDistance() {
		float distance = 0;
		if (points != null && points.size() > 0) {
			float[] results = new float[1];
			/*
			 * Log.i(MapRoute.class.getName(), "Amount of points = " +
			 * poly.size());
			 */
			for (int i = 0; i <= points.size() - 2; i++) {
				Location.distanceBetween(points.get(i).getLatitudeE6() / 1E6,
						points.get(i).getLongitudeE6() / 1E6, points.get(i + 1)
								.getLatitudeE6() / 1E6, points.get(i + 1)
								.getLongitudeE6() / 1E6, results);
				if (results != null && results.length >= 0) {
					distance += results[results.length - 1];
					/*
					 * Log.i(MapRoute.class.getName(), "Distance between " +
					 * poly.get(i).getLatitudeE6()/ 1E6 + "," +
					 * poly.get(i).getLongitudeE6()/ 1E6 + " and " + poly.get(i
					 * + 1).getLatitudeE6()/ 1E6+ "," + poly.get(i +
					 * 1).getLongitudeE6() / 1E6+ " is " +
					 * results[results.length - 1]);
					 */
				}
			}
		}
		return (float) Math.ceil(distance);
	}

	public List<GeoPoint> getPoints() {
		return points;
	}

	// http://blog.synyx.de/2010/06/routing-driving-directions-on-android-part-1-get-the-route/
	private String getUrl(double srcLat, double srcLong, double destLat,
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
}
