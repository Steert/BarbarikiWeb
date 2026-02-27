import { useState, useEffect } from "react";
import { getOrdersByPage } from "../../services/api";
import "./OrdersTable.css";
import { FaMedkit } from "react-icons/fa";

function OrdersTable() {
  const [page, setPage] = useState(1);
  const [orders, setOrders] = useState([]);
  const [totalPages, setTotalPages] = useState(0);

  const [isLoading, setIsLoading] = useState(true);

  const onChangePage = (newPage) => {
    const parsedPage = Number(newPage);

    if (!parsedPage || parsedPage < 1) {
      setPage(1);
      return;
    }

    if (totalPages > 0 && parsedPage > totalPages) {
      setPage(totalPages);
      return;
    }

    setPage(parsedPage);
  };

  const onFetchOrders = async () => {

    if (page === 1) {
      setIsLoading(true);
    }

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
    onFetchOrders();
  }, [page]);

  if (isLoading) {
    return (
      <div className="loader-container">
        <div className="spinner"></div>
        <span>Loading Data...</span>
      </div>
    );
  }

  if (orders.length === 0) {
    return (
      <div className="empty-state-container">
        <div className="empty-state-message">
          No orders yet
          <FaMedkit className="medkit-icon" />
        </div>
        <button onClick={() => onFetchOrders()} className="btn-primary">
          Click
        </button>
      </div>
    );
  }

  return (
    <>
      <table>
        <thead className="table-header">
          <tr>
            <th>Latitude</th>
            <th>Longitude</th>
            <th>Subtotal</th>
          </tr>
        </thead>
        <tbody>
          {orders.map((order) => (
            <tr key={order.id}>
              <td>{order.latitude}</td>
              <td>{order.longitude}</td>
              <td>{order.subtotal}</td>
            </tr>
          ))}
        </tbody>
      </table>

      <div className="pagination-container">
        <div className="pagination-controls">
          <span className="pagination-info">
            Сторінка {page} з {totalPages}
          </span>
          <div className="pagination-buttons">
            <button
              className="btn-pagination"
              disabled={page <= 1}
              onClick={() => setPage((prev) => prev - 1)}
            >
              Previous
            </button>

            <button
              className="btn-pagination"
              disabled={page >= totalPages}
              onClick={() => setPage((prev) => prev + 1)}
            >
              Next
            </button>
          </div>
        </div>

        <div className="page-jump-box">
          <label htmlFor="page"> Введіть номер сторінки</label>
          <input
            className="page-jump-input"
            type="number"
            min="1"
            max={totalPages}
            value={page}
            onChange={(e) => onChangePage(e.target.value)}
          />
        </div>
      </div>
    </>
  );
}

export default OrdersTable;
