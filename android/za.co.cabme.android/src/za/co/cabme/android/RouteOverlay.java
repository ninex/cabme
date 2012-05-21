package za.co.cabme.android;

import java.util.List;

import android.content.Context;
import android.graphics.Canvas;
import android.graphics.Color;
import android.graphics.Paint;
import android.graphics.Path;
import android.graphics.Point;

import com.google.android.maps.GeoPoint;
import com.google.android.maps.MapView;
import com.google.android.maps.Overlay;
import com.google.android.maps.Projection;

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