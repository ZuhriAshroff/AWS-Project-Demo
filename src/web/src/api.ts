export interface PresignUploadResponse {
    id: string;
    objectKey: string;
    uploadUrl: string;
    expiresInSeconds: number;
}

export interface ImageMetadata {
    id: string;
    objectKey: string;
    fileName: string;
    contentType: string;
    sizeBytes: number;
    createdAtUtc: string;
}

export interface PresignDownloadResponse {
    downloadUrl: string;
    expiresInSeconds: number;
}

export async function presignUpload(fileName: string, contentType: string, sizeBytes: number): Promise<PresignUploadResponse> {
    const res = await fetch('/api/images/presign-upload', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ fileName, contentType, sizeBytes })
    });
    if (!res.ok) {
        const err = await res.json().catch(() => ({ error: 'Request failed' }));
        throw new Error(err.error || 'Failed to get upload URL');
    }
    return res.json();
}

export async function uploadToS3(uploadUrl: string, file: File): Promise<void> {
    const res = await fetch(uploadUrl, {
        method: 'PUT',
        headers: { 'Content-Type': file.type },
        body: file
    });
    if (!res.ok) throw new Error('Failed to upload file to S3');
}

export async function confirmUpload(data: {
    id: string;
    objectKey: string;
    fileName: string;
    contentType: string;
    sizeBytes: number;
}): Promise<void> {
    const res = await fetch('/api/images/confirm', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data)
    });
    if (!res.ok) {
        const err = await res.json().catch(() => ({ error: 'Request failed' }));
        throw new Error(err.error || 'Failed to confirm upload');
    }
}

export async function listImages(): Promise<ImageMetadata[]> {
    const res = await fetch('/api/images');
    if (!res.ok) throw new Error('Failed to list images');
    return res.json();
}

export async function presignDownload(id: string): Promise<PresignDownloadResponse> {
    const res = await fetch(`/api/images/${id}/presign-download`);
    if (!res.ok) throw new Error('Failed to get download URL');
    return res.json();
}
