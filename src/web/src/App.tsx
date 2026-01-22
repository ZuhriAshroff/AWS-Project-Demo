import { useState } from 'react';
import { Upload } from './components/Upload';
import { Gallery } from './components/Gallery';

function App() {
    const [refreshTrigger, setRefreshTrigger] = useState(0);

    const handleUploadComplete = () => {
        setRefreshTrigger((prev) => prev + 1);
    };

    return (
        <div className="container">
            <h1>Image Gallery</h1>
            <Upload onUploadComplete={handleUploadComplete} />
            <Gallery refreshTrigger={refreshTrigger} />
        </div>
    );
}

export default App;
