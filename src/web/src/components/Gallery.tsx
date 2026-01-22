import { useState, useEffect } from 'react';
import { listImages, presignDownload, ImageMetadata } from '../api';

interface Props {
    refreshTrigger: number;
}

export function Gallery({ refreshTrigger }: Props) {
    const [images, setImages] = useState<ImageMetadata[]>([]);
    const [thumbnails, setThumbnails] = useState<Record<string, string>>({});
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        loadImages();
    }, [refreshTrigger]);

    const loadImages = async () => {
        setLoading(true);
        setError(null);
        try {
            const data = await listImages();
            setImages(data);
            // Load thumbnails for each image
            const thumbs: Record<string, string> = {};
            await Promise.all(
                data.map(async (img) => {
                    try {
                        const { downloadUrl } = await presignDownload(img.id);
                        thumbs[img.id] = downloadUrl;
                    } catch {
                        // Skip failed thumbnails
                    }
                })
            );
            setThumbnails(thumbs);
        } catch (err) {
            setError(err instanceof Error ? err.message : 'Failed to load images');
        } finally {
            setLoading(false);
        }
    };

    const handleDownload = async (image: ImageMetadata) => {
        try {
            const { downloadUrl } = await presignDownload(image.id);
            window.open(downloadUrl, '_blank');
        } catch (err) {
            alert('Failed to get download URL');
        }
    };

    const formatSize = (bytes: number) => {
        if (bytes < 1024) return `${bytes} B`;
        if (bytes < 1024 * 1024) return `${(bytes / 1024).toFixed(1)} KB`;
        return `${(bytes / (1024 * 1024)).toFixed(1)} MB`;
    };

    const formatDate = (iso: string) => {
        return new Date(iso).toLocaleDateString(undefined, {
            year: 'numeric',
            month: 'short',
            day: 'numeric',
            hour: '2-digit',
            minute: '2-digit'
        });
    };

    return (
        <section>
            <h2>Gallery</h2>
            {loading && <div className="status loading">Loading images...</div>}
            {error && <div className="status error">{error}</div>}
            {!loading && !error && images.length === 0 && (
                <div className="empty">No images yet. Upload one above!</div>
            )}
            <div className="gallery">
                {images.map((image) => (
                    <div key={image.id} className="card">
                        {thumbnails[image.id] ? (
                            <img src={thumbnails[image.id]} alt={image.fileName} />
                        ) : (
                            <div style={{ height: 180, background: '#333', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
                                Loading...
                            </div>
                        )}
                        <div className="card-body">
                            <h3 title={image.fileName}>{image.fileName}</h3>
                            <p>{formatSize(image.sizeBytes)} • {formatDate(image.createdAtUtc)}</p>
                            <button className="btn" onClick={() => handleDownload(image)}>
                                View / Download
                            </button>
                        </div>
                    </div>
                ))}
            </div>
        </section>
    );
}
