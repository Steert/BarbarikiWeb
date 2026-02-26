import axios from "axios";

const API = import.meta.env.VITE_API_URL;
const MAPBOX_TOKEN = import.meta.env.VITE_MAP_BOX_TOKEN;

export const importCsvFile = async (file) => {
  const data = new FormData();
  data.append("file", file);
  return axios.post(`${API}/import`, data, {
    headers: { "Content-Type": "multipart/form-data" },
  });
};

export const createOrderApi = async (orderData) => {
  return axios.post(API, orderData);
};

export const checkIsNewYork = async (lat, lon) => {
  const geoRes = await axios.get(
    `https://nominatim.openstreetmap.org/reverse`,
    {
      params: { format: "jsonv2", lat, lon, zoom: 18 },
      headers: { "Accept-Language": "en" },
    },
  );
  const address = geoRes.data?.address;

  if (!address || address.state !== "New York") {
    throw new Error(
      `Location must be in New York State! Your location is: ${address?.state || "Unknown"}`,
    );
  }
  return true;
};

export const checkIsNotWater = async (lat, lon) => {
  const mapboxRes = await axios.get(
    `https://api.mapbox.com/v4/mapbox.mapbox-streets-v8/tilequery/${lon},${lat}.json`,
    {
      params: { radius: 10, limit: 5, access_token: MAPBOX_TOKEN },
    },
  );
  const features = mapboxRes.data.features;
  const isWater = features.some(
    (f) => f.properties.tilequery.layer === "water",
  );

  if (isWater) {
    throw new Error("Destination is in the water! Drones cannot land there.");
  }
  return true;
};

export const getOrdersByPage = async (page) => {
  const response = await axios.get(`${API}?pageNumber=${page}&pageSize=10`);

  return response;

};