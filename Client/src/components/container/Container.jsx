import { useState, useRef } from "react";
import "./Container.css";
import { MdDeleteForever } from "react-icons/md";
import axios from "axios";

const Container = () => {
  const [formData, setFormData] = useState({
    latitude: "",
    longitude: "",
    subtotal: "",
  });
  const [isCorrectLatitude, setIsCorrectLatitude] = useState(true);
  const [isCorrectLongitude, setIsCorrectLongitude] = useState(true);
  const [isCorrectSubtotal, setIsCorrectSubtotal] = useState(true);
  const [fileName, setFileName] = useState("");
  const fileInputRef = useRef(null);
  const API = import.meta.env.VITE_API_URL;

  const handleFileChange = (event) => {
    const file = event.target.files[0];
    if (file) setFileName(file.name);
  };

  const handleResetFile = () => {
    setFileName("");
    if (fileInputRef.current) fileInputRef.current.value = "";
  };

  const handleChange = (event) => {
    const { name, value } = event.target;
    const numValue = parseFloat(value);

    if (name === "latitude") {
      setIsCorrectLatitude(value === "" || !isNaN(numValue));
    } else if (name === "longitude") {
      setIsCorrectLongitude(value === "" || !isNaN(numValue));
    } else if (name === "subtotal") {
      setIsCorrectSubtotal(value === "" || (!isNaN(numValue) && numValue > 0));
    }

    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  const isFormValid =
    formData.latitude !== "" &&
    formData.longitude !== "" &&
    formData.subtotal !== "" &&
    isCorrectLatitude &&
    isCorrectLongitude &&
    isCorrectSubtotal;

  const handleSubmitImport = (event) => {
    event.preventDefault();
    if (!fileInputRef.current.files[0]) return;

    const data = new FormData();
    data.append("file", fileInputRef.current.files[0]);

    axios
      .post(`${API}/import`, data, {
        headers: { "Content-Type": "multipart/form-data" },
      })
      .then(() => {
        handleResetFile();
        alert("Imported!");
      })
      .catch(() => alert("Import Error!"));
  };

  const handleCreateOrder = async (event) => {
    event.preventDefault();

    const MAPBOX_TOKEN = import.meta.env.VITE_MAPBOX_TOKEN;

    try {
      const geoRes = await axios.get(
        `https://nominatim.openstreetmap.org/reverse`,
        {
          params: {
            format: "jsonv2",
            lat: formData.latitude,
            lon: formData.longitude,
            zoom: 18,
          },
          headers: { "Accept-Language": "en" },
        },
      );

      const address = geoRes.data?.address;
      if (!address || address.state !== "New York") {
        alert(`Error: Location must be in New York State!`);
        return;
      }

      const mapboxRes = await axios.get(
        `https://api.mapbox.com/v4/mapbox.mapbox-streets-v8/tilequery/${formData.longitude},${formData.latitude}.json`,
        {
          params: {
            radius: 10,
            limit: 5,
            access_token: import.meta.env.MAP_BOX_TOKEN,
          },
        },
      );

      const features = mapboxRes.data.features;
      const isWater = features.some(
        (f) => f.properties.tilequery.layer === "water",
      );

      if (isWater) {
        alert("Error: Destination is in the water! Drones cannot land there.");
        return;
      }

      await axios.post(API, {
        latitude: parseFloat(formData.latitude),
        longitude: parseFloat(formData.longitude),
        subtotal: parseFloat(formData.subtotal),
      });

      setFormData({ latitude: "", longitude: "", subtotal: "" });
      alert("Order Created Successfully in NY State!");
    } catch (error) {
      console.error("Validation error:", error);
      alert("Error during location validation. Check your console.");
    }
  };

  return (
    <div className="container">
      <div className="row">
        <div className="col">
          <div className="content">
            <h2 className="content-text">Manual Order</h2>
            <form onSubmit={handleCreateOrder}>
              <div className="form-item">
                <label>Latitude:</label>
                <input
                  name="latitude"
                  type="text"
                  value={formData.latitude}
                  placeholder="40.7128"
                  onChange={handleChange}
                />
              </div>
              {!isCorrectLatitude && formData.latitude !== "" && (
                <p className="error-text">Not valid data</p>
              )}

              <div className="form-item">
                <label>Longitude:</label>
                <input
                  name="longitude"
                  type="text"
                  value={formData.longitude}
                  placeholder="-74.0060"
                  onChange={handleChange}
                />
              </div>
              {!isCorrectLongitude && formData.longitude !== "" && (
                <p className="error-text">Not valid data</p>
              )}

              <div className="form-item">
                <label>Subtotal:</label>
                <input
                  name="subtotal"
                  type="text"
                  value={formData.subtotal}
                  placeholder="100.00"
                  onChange={handleChange}
                />
              </div>
              {!isCorrectSubtotal && formData.subtotal !== "" && (
                <p className="error-text">Not valid data</p>
              )}

              <button type="submit" className="btn" disabled={!isFormValid}>
                Create order
              </button>
            </form>
          </div>
        </div>

        <div className="col">
          <div className="content">
            <h2 className="content-text">Import CSV</h2>
            <div className="import-box">
              <p style={{ color: "#64748b", fontSize: "14px" }}>
                {fileName
                  ? "Selected file:"
                  : "Drop your CSV file here or click to browse"}
              </p>
              {fileName && (
                <div className="file-name-box">
                  <p className="file-name">📄 {fileName}</p>
                  <MdDeleteForever
                    onClick={handleResetFile}
                    className="delete-file-btn"
                  />
                </div>
              )}
              <div>
                <input
                  ref={fileInputRef}
                  type="file"
                  id="file-upload"
                  className="hidden-input"
                  accept=".csv"
                  onChange={handleFileChange}
                />
                <label htmlFor="file-upload" className="custom-file-upload">
                  {fileName ? "Change File" : "Choose File"}
                </label>
              </div>
              <button
                className="btn"
                disabled={!fileName}
                onClick={handleSubmitImport}
              >
                Start Import Process
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Container;
