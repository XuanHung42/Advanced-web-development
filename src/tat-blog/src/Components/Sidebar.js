import React from 'react'
import SearchForm from './SearchForm';
import CategoriesWidget from './CategoriesWidget';
import PostsFeatureWidget from './PostsFeatureWidget';
import PostRadomWidget from './PostsRadomWidget';
import TagsCloudWidget from './TagsCloudWidget';
import BestAuthorsWidget from './BestAuthorsWIdget';

const Sidebar = ()=> {
    return(
        <div className='pt-4 ps-2'>
            <SearchForm/>
           <CategoriesWidget/>
           <PostsFeatureWidget/>
           <PostRadomWidget/>
           <TagsCloudWidget/>
           <BestAuthorsWidget/>

        </div>
    )
}
export default Sidebar;
