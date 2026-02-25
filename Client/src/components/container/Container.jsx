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
  const [fileName, setFileName] = useState("");
  const fileInputRef = useRef(null);
  const API = import.meta.env.VITE_API_URL;

  const handleFileChange = (event) => {
    const file = event.target.files[0];
    if (file) {
      setFileName(file.name);
    }
  };

  const handleResetFile = () => {
    setFileName("");
    if (fileInputRef.current) {
      fileInputRef.current.value = "";
    }
  };

  const handleChange = (event) => {
    const { name, value } = event.target;
    setFormData((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  const handleSubmit = (event) => {
    event.preventDefault();
  };

  const handleCreate = (event) => {
    event.preventDefault();

    axios
      .post(API, formData)
      .then((response) => {
        console.log(response.data);
        console.log("Order created successfully");
      })
      .catch((error) => {
        console.log("Error creating order");
        console.error(error);
      });

  }

  return (
    <div className="container">
      <div className="row">
        <div className="col">
          <div className="content">
            <h2 className="content-text">Manual Order</h2>

            <form onSubmit={handleSubmit}>
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

              <button type="submit" className="btn" onClick={(e) => handleCreate(e)}>
                Create order
              </button>
            </form>
          </div>
        </div>

        <div className="col">
          <div className="content">
            <h2 className="content-text">Import CSV</h2>
            <div className="import-box">
              <p
                style={{
                  color: "#64748b",
                  fontSize: "14px",
                }}
              >
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

              <button className="btn" disabled={!fileName}>
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
