import React from 'react';
import './Footer.css';

const Footer = () => {
    return (
      <footer className="footer">
        <div className="footer-content">
          <div className="footer-section">
            <h4 className="footer-logo">Instant Wellness Kits</h4>
            <p>Instant reset for stressful days. Delivered by drone.</p>
          </div>

          <div className="footer-section">
            <p className="footer-label">Operations</p>
            <div className="status-badge">
              <span className="dot"></span> Systems Online
            </div>
            <p className="license-text">NYS License #NY-2026-IWK</p>
          </div>

          <div className="footer-section">
            <p className="footer-label">Legal</p>
            <div className="footer-links">
              <a href="#">Privacy Policy</a>
              <a href="#">Tax Jurisdictions</a>
            </div>
          </div>
        </div>
        <div className="footer-bottom">
          © 2026 Instant Wellness Kits Inc. All rights reserved.
        </div>
      </footer>
    );
}

export default Footer;
