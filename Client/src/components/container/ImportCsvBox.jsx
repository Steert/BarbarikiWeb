import { useState, useRef } from "react";
import { MdDeleteForever } from "react-icons/md";
import { importCsvFile } from "../../services/api";
import { showSuccess, showError } from "../../utils/Alerts";

const ImportCsvBox = () => {
  const [fileName, setFileName] = useState("");
  const fileInputRef = useRef(null);

  const handleFileChange = (event) => {
    const file = event.target.files[0];
    if (file) setFileName(file.name);
  };

  const handleResetFile = () => {
    setFileName("");
    if (fileInputRef.current) fileInputRef.current.value = "";
  };

  const handleSubmitImport = async (event) => {
    event.preventDefault();
    const file = fileInputRef.current.files[0];
    if (!file) return;

    try {
      await importCsvFile(file);
      handleResetFile();
      showSuccess("Successfully imported!");
    } catch (error) {
      console.error(error);
      showError("Import CSV Error", "Something went wrong, please try again");
    }
  };

  return (
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
  );
};

export default ImportCsvBox;
