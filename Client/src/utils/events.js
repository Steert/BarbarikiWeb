export const refreshOrdersEvent = () => {
  window.dispatchEvent(new Event("refresh-orders"));
};
