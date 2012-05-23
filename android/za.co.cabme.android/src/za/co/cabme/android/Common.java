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
import org.apache.http.entity.ByteArrayEntity;
import org.apache.http.impl.client.DefaultHttpClient;
import org.apache.http.params.BasicHttpParams;
import org.apache.http.params.HttpConnectionParams;
import org.apache.http.params.HttpParams;

import android.util.Log;

public class Common {
	//Flags
	public static final String NEWBOOKING_FLAG = "za.co.cabme.android.NewBookingFlag";
	public static final String REVIEW_FLAG = "za.co.cabme.android.ReviewFlag";
	public static final String ADDRESS_LOCKED_FLAG = "za.co.cabme.android.AddressLockedFlag";
	public static final String FROMADDR_FLAG = "za.co.cabme.android.FromAddressFlag";
	public static final String TOADDR_FLAG = "za.co.cabme.android.ToAddressFlag";
	public static final String FROMLAT_FLAG = "za.co.cabme.android.FromLatFlag";
	public static final String FROMLONG_FLAG = "za.co.cabme.android.FromLongFlag";
	public static final String TOLAT_FLAG = "za.co.cabme.android.ToLatFlag";
	public static final String TOLONG_FLAG = "za.co.cabme.android.ToLongFlag";
	public static final String TAXI_FLAG = "za.co.cabme.android.TaxiFlag";
	public static final String BOOKING_FLAG = "za.co.cabme.android.BookingFlag";
	//Codes
	public static final int PICK_ADDRESS_FROM = 11;
	public static final int PICK_ADDRESS_TO = 12;
	public static final int PICK_TAXI = 13;
	public static final int TIME_DIALOG_ID = 0;
	public static final int DATEPICKER_DIALOG_ID = 1;
	public static final int NUMPEOPLE_DIALOG_ID = 2;
	
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
				Log.e(Common.class.toString(), "Failed to download file");
			}
		} catch (ClientProtocolException e) {
			e.printStackTrace();
		} catch (IOException e) {
			e.printStackTrace();
		}
		return builder.toString();
	}
	public static String postRESTurl(String url, String jsonRequest){
		StringBuilder builder = new StringBuilder();
		try {
			int TIMEOUT_MILLISEC = 0;  // = 10 seconds
			HttpParams httpParams = new BasicHttpParams();
			HttpConnectionParams.setConnectionTimeout(httpParams, TIMEOUT_MILLISEC);
			HttpConnectionParams.setSoTimeout(httpParams, TIMEOUT_MILLISEC);
			HttpClient client = new DefaultHttpClient(httpParams);
			HttpPost request = new HttpPost(url);
			request.setEntity(new ByteArrayEntity(jsonRequest.toString().getBytes("UTF8")));
			HttpResponse response = client.execute(request);
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
				Log.e(Common.class.toString(), "Failed to download file");
			}
		} catch (ClientProtocolException e) {
			e.printStackTrace();
		} catch (IOException e) {
			e.printStackTrace();
		}
		return builder.toString();
	}
}
