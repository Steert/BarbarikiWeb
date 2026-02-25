import React from "react";
import "./Header.css";

const Header = () => {
  return (
    <div className="header-container">
        <div className="info">
          <div>
            <img src="../src/assets/header-logo.png" alt="logo wellness kit image" className="logo" />
          </div>
          <div className="text-info">
            <p className="text">Instant</p>
            <p className="text">Wellness Kits</p>
          </div>
        </div>
    </div>
  );
};

export default Header;
