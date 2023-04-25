import { configureStore } from '@reduxjs/toolkit';
import { reducer } from './Redux';

const store = configureStore({
reducer: {
postFilter: reducer,
},
});
export default store