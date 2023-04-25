import axios from "axios";
export async function getTagsCloud(){
    try{
        const response= await axios.get(`https://localhost:7278/api/tags`);
        const data = response.data;
        if(data.isSuccess){
            return data.result;
        }
        else
            return null;
    }catch(error){
            console.log('Error', error.message);
            return null;
    }
}
export async function getTagBySlug(urlSlug = '', pageSize = 3, pageNumber = 1){
    try {
      const response = await axios.get(`https://localhost:7278/api/tags/${urlSlug}?PageSize=${pageSize}&PageNumber=${pageNumber}`);
      
      const data = response.data
      if (data.isSuccess)
        return data.result;
      else
        return null;
        
    } catch (error) {
      
    }
  }