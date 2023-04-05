import React from 'react'
import SearchForm from './SearchForm';
import CategoriesWidget from './CategoriesWidget';

const Sidebar = ()=> {
    return(
        <div className='pt-4 ps-2'>
            <SearchForm/>
           <CategoriesWidget/>

        </div>
    )
}
export default Sidebar;
