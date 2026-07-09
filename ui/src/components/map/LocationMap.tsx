'use client';

import { MapContainer, TileLayer, Marker, Popup } from 'react-leaflet';
import 'leaflet/dist/leaflet.css';
import L from 'leaflet';
import type { GourmetLocationLog } from '@/types/api';
import { formatDateTime } from '@/lib/utils/date';

// Leaflet のデフォルトアイコン設定（webpack バンドル時のパス問題を回避）
delete (L.Icon.Default.prototype as unknown as Record<string, unknown>)._getIconUrl;
L.Icon.Default.mergeOptions({
  iconRetinaUrl: 'https://unpkg.com/leaflet@1.9.4/dist/images/marker-icon-2x.png',
  iconUrl: 'https://unpkg.com/leaflet@1.9.4/dist/images/marker-icon.png',
  shadowUrl: 'https://unpkg.com/leaflet@1.9.4/dist/images/marker-shadow.png',
});

type Props = {
  logs: GourmetLocationLog[];
};

export function LocationMap({ logs }: Props): React.ReactElement {
  const first = logs[0];
  const center: [number, number] = first
    ? [first.lat, first.lng]
    : [35.6812, 139.7671]; // 東京をデフォルト中心に

  return (
    <MapContainer center={center} zoom={12} className="h-72 w-full rounded-lg sm:h-96">
      <TileLayer
        attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a>'
        url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
      />
      {logs.map((log) => {
        if (!isValidCoord(log.lat, log.lng)) return null;
        return (
          <Marker key={log.id} position={[log.lat, log.lng]}>
            <Popup>
              <div className="text-xs">
                <p>緯度: {log.lat}</p>
                <p>経度: {log.lng}</p>
                <p>{formatDateTime(log.createdAt)}</p>
              </div>
            </Popup>
          </Marker>
        );
      })}
    </MapContainer>
  );
}

function isValidCoord(lat: number, lng: number): boolean {
  return lat >= -90 && lat <= 90 && lng >= -180 && lng <= 180;
}
