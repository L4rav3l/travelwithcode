import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import { BrowserRouter, Route, Routes} from 'react-router-dom';
import Login from './Login';
import Menu from './Dashboard/Menu';
import Add from './Dashboard/Admin/Add';

createRoot(document.getElementById('root')).render(
    <StrictMode>
        <BrowserRouter>
            <Routes>
                <Route path="/" element={<Login />} />
                <Route path="/dashboard" element={<Menu />} />
                <Route path="/dashboard/add" element={<Add />} />
            </Routes>
        </BrowserRouter>
    </StrictMode>
)