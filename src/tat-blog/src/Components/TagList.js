import {Link} from 'react-router-dom'
const TagList =({tagList})=> {

    if(tagList && Array.isArray(tagList) && tagList.length>0)
        return(
        <>
        {tagList.map((item, index)=> {
        return(
            <Link to={`/blog/tag?slug=${item.name}`}
            title={item.name}
            key={index}
            className='btn btn-sm btn-outline-secondary me-1'>
                {item.name}
            </Link>
        );
        })}
        </>
        );
        else
            return(
                <></>
            );

            };
export default TagList
   
