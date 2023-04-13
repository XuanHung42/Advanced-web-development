
import './App.css';

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
import AdminLayout from './Pages/Admin/Layout';
import * as adminIndex from './Pages/Admin/Index';
import Authors from './Pages/Admin/Authors'; 
import Categories from './Pages/Admin/Categories'; 
import Posts from './Pages/Admin/Post/Posts'; 
import Comments from './Pages/Admin/Comments'; 
import Tags from './Pages/Admin/Tags'; 
import PostsByTag from './Pages/PostsByTag';
import PostDetail from './Pages/PostDetail';
import PostByCateSlug from './Pages/PostByCateSlug';
import PostByAuthor from './Pages/PostByAuthor';




function App() {
  return (
    // <div>
      <Router>
    
              <Routes> 
                <Route path='/' element={<Layout />}>
                  <Route path='/' element={<Index />}/>
                  <Route path='blog' element={<Index/>}/>
                  <Route path='blog/contact' element={<Contact/>}/>
                  <Route path='blog/about' element={<About/>}/>
                  <Route path='blog/RSS' element={<Rss/>}/>
                  <Route path='/blog/tag/:tagSlug' element={<PostsByTag/>}/>
                  <Route path="/post/:slug" element={<PostDetail/>}/>
                  <Route path="/blog/category/:cateSlug" element={<PostByCateSlug/>}/>
                  <Route path="/blog/author/:authorSlug" element={<PostByAuthor/>}/>

                 
                </Route>
                <Route path='/admin' element={<AdminLayout/>}>
                <Route path='/admin' element={<adminIndex.default/>}/>
                <Route path='/admin/authors' element={<Authors/>}/>
                <Route path='/admin/categories' element={<Categories/>}/>
                <Route path='/admin/comments' element={<Comments />}/>
                <Route path='/admin/posts' element={<Posts/>}/>
                <Route path='/admin/tags' element={<Tags/>}/>

                </Route>

                

              </Routes>
         
        <Footer />
      </Router>
   
  );
 }

export default App;
