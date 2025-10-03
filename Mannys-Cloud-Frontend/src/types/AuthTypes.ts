export type AuthState = 'register' | 'login'

export type AuthCredentials = {
    fullName: string | null;
    email: string;
    password: string;
}