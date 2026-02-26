import "./Container.css";
import ManualOrderForm from "./ManualOrderForm";
import ImportCsvBox from "./ImportCsvBox";
import ShowOrdersLIst from "./OrdersList";

const Container = () => {
  return (
    <div className="container">
      <div className="row">
        <div className="col">
          <ManualOrderForm />
        </div>
        <div className="col">
          <ImportCsvBox />
        </div>
      </div>
      <div className="row">
        <div className="col">
          <ShowOrdersLIst />
        </div>
      </div>
    </div>
  );
};

export default Container;
