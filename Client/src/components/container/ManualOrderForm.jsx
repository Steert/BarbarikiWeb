import { useState } from "react";
import {
  createOrderApi,
  checkIsNewYork,
  checkIsNotWater,
} from "../../services/api";
import { showSuccess, showError } from "../../utils/Alerts";
import { refreshOrdersEvent } from "../../utils/events";

const ManualOrderForm = () => {
  const [formData, setFormData] = useState({
    latitude: "",
    longitude: "",
    subtotal: "",
  });
  const [isCorrectLatitude, setIsCorrectLatitude] = useState(true);
  const [isCorrectLongitude, setIsCorrectLongitude] = useState(true);
  const [isCorrectSubtotal, setIsCorrectSubtotal] = useState(true);

  const handleChange = (event) => {
    const { name, value } = event.target;
    const numValue = parseFloat(value);

    if (name === "latitude")
      setIsCorrectLatitude(value === "" || !isNaN(numValue));
    else if (name === "longitude")
      setIsCorrectLongitude(value === "" || !isNaN(numValue));
    else if (name === "subtotal")
      setIsCorrectSubtotal(value === "" || (!isNaN(numValue) && numValue > 0));

    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  const isFormValid =
    formData.latitude !== "" &&
    formData.longitude !== "" &&
    formData.subtotal !== "" &&
    isCorrectLatitude &&
    isCorrectLongitude &&
    isCorrectSubtotal;

  const handleCreateOrder = async (event) => {
    event.preventDefault();
    const lat = parseFloat(formData.latitude);
    const lon = parseFloat(formData.longitude);
    const subtotal = parseFloat(formData.subtotal);

    try {
      await checkIsNewYork(lat, lon);
      await checkIsNotWater(lat, lon);
      await createOrderApi({ latitude: lat, longitude: lon, subtotal });

      refreshOrdersEvent();

      setFormData({ latitude: "", longitude: "", subtotal: "" });
      showSuccess("Success!", "Order created!");
    } catch (error) {
      console.error("Validation error:", error);
      showError(
        "Delivery Error",
        error.message || "Error during location validation.",
      );
    }
  };

  return (
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
  );
};

export default ManualOrderForm;
