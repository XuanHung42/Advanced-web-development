import React from 'react'
import SearchForm from './SearchForm';
import CategoriesWidget from './CategoriesWidget';
import PostsFeatureWidget from './PostsFeatureWidget';
import PostRadomWidget from './PostsRadomWidget';

const Sidebar = ()=> {
    return(
        <div className='pt-4 ps-2'>
            <SearchForm/>
           <CategoriesWidget/>
           <PostsFeatureWidget/>
           <PostRadomWidget/>

        </div>
    )
}
export default Sidebar;
