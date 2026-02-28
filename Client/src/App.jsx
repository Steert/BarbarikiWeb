import Header from './components/header/Header'
import Container from './components/container/Container'
import Footer from './components/footer/Footer'
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";

const queryClient = new QueryClient();

function App() {

  return (
    <>
    <QueryClientProvider client={queryClient}>
      <Header/>
      <Container/>
      <Footer/>
    </QueryClientProvider>
    </>
  )
}

export default App
