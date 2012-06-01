package za.co.cabme.android;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;

import org.apache.http.HttpEntity;
import org.apache.http.HttpResponse;
import org.apache.http.StatusLine;
import org.apache.http.client.ClientProtocolException;
import org.apache.http.client.HttpClient;
import org.apache.http.client.methods.HttpGet;
import org.apache.http.client.methods.HttpPost;
import org.apache.http.client.params.ClientPNames;
import org.apache.http.client.params.CookiePolicy;
import org.apache.http.entity.StringEntity;
import org.apache.http.impl.client.DefaultHttpClient;
import org.apache.http.params.BasicHttpParams;
import org.apache.http.params.HttpConnectionParams;
import org.apache.http.params.HttpParams;

import android.util.Log;

public class Common {
	// Flags
	public static final String NEWBOOKING_FLAG = "za.co.cabme.android.NewBookingFlag";
	public static final String REVIEW_FLAG = "za.co.cabme.android.ReviewFlag";
	public static final String MAPFROM_FLAG = "za.co.cabme.android.MapFromFlag";
	public static final String ADDRESS_LOCKED_FLAG = "za.co.cabme.android.AddressLockedFlag";
	public static final String TAXI_FLAG = "za.co.cabme.android.TaxiFlag";
	public static final String BOOKING_FLAG = "za.co.cabme.android.BookingFlag";
	// Codes
	public static final int PICK_ADDRESS_FROM = 11;
	public static final int PICK_ADDRESS_TO = 12;
	public static final int PICK_TAXI = 13;
	public static final int TIME_DIALOG_ID = 0;
	public static final int DATEPICKER_DIALOG_ID = 1;
	public static final int NUMPEOPLE_DIALOG_ID = 2;
	public static final int NOTIFY_BOOKINGCREATED = 100;

	public static boolean isNullOrEmpty(String input) {
		return input == null || input.trim().length() == 0;
	}

	public static String queryRESTurl(String url) {
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
				Log.e(Common.class.toString(), "Http result:" + statusCode);
			}
		} catch (ClientProtocolException e) {
			e.printStackTrace();
		} catch (IOException e) {
			e.printStackTrace();
		}
		return builder.toString();
	}

	public static String postRESTurl(String url, String jsonRequest) {
		StringBuilder builder = new StringBuilder();
		try {
			int TIMEOUT_MILLISEC = 10000;
			HttpParams httpParams = new BasicHttpParams();
			HttpConnectionParams.setConnectionTimeout(httpParams,TIMEOUT_MILLISEC);
			HttpConnectionParams.setSoTimeout(httpParams, TIMEOUT_MILLISEC);
			
			HttpClient client = new DefaultHttpClient(httpParams);
			client.getParams().setParameter(ClientPNames.COOKIE_POLICY, CookiePolicy.RFC_2109);
			
			HttpPost post = new HttpPost(url);
			post.setHeader("Accept", "application/json");
			post.setHeader("Content-Type", "application/json");
			
			StringEntity ent = new StringEntity(jsonRequest);
			post.setEntity(ent);
			
			HttpResponse response = client.execute(post);
			// Read the response
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
				Log.e(Common.class.toString(), "Post to:" + url
						+ ",\nHttp result:" + statusCode + ", json request:"
						+ jsonRequest);
				return null;
			}
		} catch (ClientProtocolException e) {
			e.printStackTrace();
		} catch (IOException e) {
			e.printStackTrace();
		}
		return builder.toString();
	}
}
