import React from 'react'
import SearchForm from './SearchForm';
import CategoriesWidget from './Blog/categories/CategoriesWidget';
import PostsFeatureWidget from './Blog/posts/PostsFeatureWidget';
import PostRadomWidget from './Blog/posts/PostsRadomWidget';
import TagsCloudWidget from './Blog/tags/TagsCloudWidget';
import BestAuthorsWidget from './Blog/authors/BestAuthorsWIdget';

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
