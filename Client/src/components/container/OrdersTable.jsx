import { useState, useEffect } from "react";
import { getOrdersByPage } from "../../services/api";
import "./OrdersTable.css";
import { FaMedkit } from "react-icons/fa";

function OrdersTable() {
  const [page, setPage] = useState(1);
  const [orders, setOrders] = useState([]);
  const [totalPages, setTotalPages] = useState(0);
  const [isLoading, setIsLoading] = useState(true);

  const onFetchOrders = async (silent = false) => {
    if (!silent) setIsLoading(true);
    try {
      const { data } = await getOrdersByPage(page);
      setOrders(data.items);
      setTotalPages(data.totalPages);
    } catch (error) {
      console.error("Fetch error:", error);
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    onFetchOrders(orders.length > 0);

    const handleRefresh = () => onFetchOrders(true);
    window.addEventListener("refresh-orders", handleRefresh);
    return () => window.removeEventListener("refresh-orders", handleRefresh);
  }, [page]);

  const onChangePage = (newPage) => {
    const parsedPage = Number(newPage);
    if (!parsedPage || parsedPage < 1) return setPage(1);
    if (totalPages > 0 && parsedPage > totalPages) return setPage(totalPages);
    setPage(parsedPage);
  };

  if (isLoading && orders.length === 0) {
    return (
      <div className="loader-container">
        <div className="spinner"></div>
        <span>Loading data...</span>
      </div>
    );
  }

  if (orders.length === 0) {
    return (
      <div className="empty-state-container">
        <div className="empty-state-message">
          No orders yet <FaMedkit className="medkit-icon" />
        </div>
        <button onClick={() => onFetchOrders()} className="btn-primary">
          Update
        </button>
      </div>
    );
  }

  return (
    <div style={{ opacity: isLoading ? 0.6 : 1, transition: "opacity 0.2s" }}>
      <div className="table-responsive">
        <table>
          <thead className="table-header">
            <tr>
              <th>Latitude</th>
              <th>Longitude</th>
              <th>Composite Tax</th>
              <th>State Rate</th>
              <th>County Rate</th>
              <th>Special</th>
              <th>Subtotal</th>
              <th>Tax Amount</th>
              <th>Total Amount</th>
            </tr>
          </thead>
          <tbody>
            {orders.map((order) => (
              <tr key={order.id}>
                <td>{order.latitude}</td>
                <td>{order.longitude}</td>
                <td>{order.composite_tax_rate}%</td>
                <td>{order.state_rate}</td>
                <td>{order.county_rate}</td>
                <td>{order.special_rates}</td>
                <td>${order.subtotal}</td>
                <td>${order.tax_amount}</td>
                <td>${order.total_amount}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      <div className="pagination-container">
        <div className="pagination-controls">
          <span className="pagination-info">
            Page {page} з {totalPages}
          </span>
          <div className="pagination-buttons">
            <button
              className="btn-pagination"
              disabled={page <= 1 || isLoading}
              onClick={() => setPage((p) => p - 1)}
            >
              Previous
            </button>
            <button
              className="btn-pagination"
              disabled={page >= totalPages || isLoading}
              onClick={() => setPage((p) => p + 1)}
            >
              Next
            </button>
          </div>
        </div>
        <div className="page-jump-box">
          <label>Page:</label>
          <input
            className="page-jump-input"
            type="number"
            value={page}
            onChange={(e) => onChangePage(e.target.value)}
          />
        </div>
      </div>
    </div>
  );
}

export default OrdersTable;
