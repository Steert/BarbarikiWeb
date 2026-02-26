import { useState, useEffect } from "react";
import { getOrdersByPage } from "../../services/api";
import "./OrdersTable.css";

function OrdersTable() {
  const [page, setPage] = useState(1);
  const [orders, setOrders] = useState([]);
  const [totalPages, setTotalPages] = useState(0);

  const onChangePage = (newPage) => {
    if (newPage < 1) newPage = 1;
    setPage(newPage);
  };

  useEffect(() => {
    async function fetchOrders() {
      try {
        const { data } = await getOrdersByPage(page);

        setOrders(data.items);
        setTotalPages(data.totalPages);
      } catch (error) {
        console.error("Fetch error:", error);
      }
    }
    fetchOrders();
  }, [page]);

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

      <div className="pagination">
        <div className="pagination-line">
          <span className="pagination-info">
            Сторінка {page} з {totalPages}
          </span>
          <div className="buttons">
            <button
              disabled={page <= 1}
              onClick={() => setPage((prev) => prev - 1)}
            >
              Previous
            </button>

            <button
              disabled={page >= totalPages}
              onClick={() => setPage((prev) => prev + 1)}
            >
              Next
            </button>
          </div>
        </div>
        <div className="manual-input">
          <label htmlFor="page"> Введіть номер сторінки</label>
          <input
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
