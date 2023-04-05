import logo from './logo.svg';
import './App.css';
import Navbar  from './Components/Navbar';
import Sidebar from './Components/Sidebar';
import Footer from './Components/Footer';
import {
  BrowserRouter as Router,
Routes,
Route,

} from 'react-router-dom';
import Index from './Pages/Index';
import Layout from './Pages/Layout';
import Contact from './Pages/Contact';
import Rss from './Pages/Rss';
import About from './Pages/About';
function App() {
  return (
    <div>
      <Router>
        <Navbar />
        <div className='container-fluid'>
          <div className='row'>
            <div className='col-9'>
              <Routes>
                <Route path='/' element={<Layout />}>
                  <Route path='/' element={<Index />}/>
                  <Route path='blog' element={<Index/>}/>
                  <Route path='blog/contact' element={<Contact/>}/>
                  <Route path='blog/about' element={<About/>}/>
                  <Route path='blog/RSS' element={<Rss/>}/>

                </Route>
              </Routes>
            </div>
            <div className='col-3 border-start'>
              <Sidebar/>
            </div>
          </div>
        </div>
        <Footer />
      </Router>
    </div>
  );
}

export default App;
