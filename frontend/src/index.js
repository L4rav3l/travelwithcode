import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import { BrowserRouter, Route, Routes} from 'react-router-dom';
import Login from './Login';
import Menu from './Dashboard/Menu';
import Add from './Dashboard/Admin/Add';
import Edit from './Dashboard/Admin/Edit';

createRoot(document.getElementById('root')).render(
    <StrictMode>
        <BrowserRouter>
            <Routes>
                <Route path="/" element={<Login />} />
                <Route path="/dashboard" element={<Menu />} />
                <Route path="/dashboard/add" element={<Add />} />
                <Route path="/dashboard/edit" element={<Edit />} />
            </Routes>
        </BrowserRouter>
    </StrictMode>
)